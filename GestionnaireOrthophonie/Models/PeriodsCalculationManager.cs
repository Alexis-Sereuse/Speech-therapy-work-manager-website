using GestionnaireOrthophonie.Models.Entities;

namespace GestionnaireOrthophonie.Models
{
    public static class PeriodsCalculationManager
    {
        public static DateTime[] CurrentPeriodBorders = Array.Empty<DateTime>();
        public static DateTime[] VisualizedPeriodBorders = Array.Empty<DateTime>();

        public const float THREE_LINES_CELL_MINIMUM_HEIGHT = 50f;
        public const float NO_END_DATE_FILLED_HOURS_COUNT = 0.5f;

        public static string GetFrenchDayName(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Lundi";
                case DayOfWeek.Tuesday:
                    return "Mardi";
                case DayOfWeek.Wednesday:
                    return "Mercredi";
                case DayOfWeek.Thursday:
                    return "Jeudi";
                case DayOfWeek.Friday:
                    return "Vendredi";
                case DayOfWeek.Saturday:
                    return "Samedi";
                case DayOfWeek.Sunday:
                    return "Dimanche";
                default:
                    return "";
            }
        }

        private static DateTime GetMondayDateByWeekDate(DateTime date)
        {
            return date.AddDays(-(((int)date.DayOfWeek) - 1));
        }

        private static DateTime[] GetCurrentPeriodFirstAndLastDates(NamedPeriod? currentPeriodType)
        {
            if (currentPeriodType == null)
                return Array.Empty<DateTime>();

            DateTime yearFirstMonday = GetMondayDateByWeekDate(new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0));

            DateTime searchPeriodStart = yearFirstMonday;
            DateTime searchPeriodEnd = yearFirstMonday.AddDays(7 * currentPeriodType.WeeksLength).AddMicroseconds(-1);

            while (true)
            {
                if (searchPeriodStart.Year > DateTime.Now.Year)
                    return Array.Empty<DateTime>();

                if (DateTime.Now >= searchPeriodStart && DateTime.Now <= searchPeriodEnd)
                    break;

                searchPeriodStart = searchPeriodEnd.AddMicroseconds(1);
                searchPeriodEnd = searchPeriodStart.AddDays(7 * currentPeriodType.WeeksLength).AddMicroseconds(-1);
            }

            return [searchPeriodStart, searchPeriodEnd];
        }

        public static void OnNextPeriodVisualizationButtonPressed(NamedPeriod? currentPeriodType)
        {
            int weeksLength = currentPeriodType != null ? currentPeriodType.WeeksLength : 1;
            VisualizedPeriodBorders[0] = VisualizedPeriodBorders[1].AddMicroseconds(1);
            VisualizedPeriodBorders[1] = VisualizedPeriodBorders[0].AddDays(7 * weeksLength).AddMicroseconds(-1);
        }

        public static void OnPreviousPeriodVisualizationButtonPressed(NamedPeriod? currentPeriodType)
        {
            int weeksLength = currentPeriodType != null ? currentPeriodType.WeeksLength : 1;
            VisualizedPeriodBorders[1] = VisualizedPeriodBorders[0].AddMicroseconds(-1);
            VisualizedPeriodBorders[0] = VisualizedPeriodBorders[1].AddDays(-7 * weeksLength).AddMicroseconds(1);
        }

        public static void OnCurrentPeriodVisualizationButtonPressed()
        {
            VisualizedPeriodBorders = [CurrentPeriodBorders[0], CurrentPeriodBorders[1]];
        }

        public static void SetCurrentAndVisualizedPeriods(NamedPeriod? currentPeriodType)
        {
            CurrentPeriodBorders = GetCurrentPeriodFirstAndLastDates(currentPeriodType);

            if (CurrentPeriodBorders.Length == 0)
            {
                DateTime startDate = GetMondayDateByWeekDate(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0));
                DateTime endDate = startDate.AddDays(7 * (currentPeriodType != null ? currentPeriodType.WeeksLength : 1)).AddMicroseconds(-1);
                CurrentPeriodBorders = [startDate, endDate];
            }

            VisualizedPeriodBorders = [CurrentPeriodBorders[0], CurrentPeriodBorders[1]];
        }

        public static Dictionary<string, string> GetSessionCellStylesAndInformation(Session session)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            DateTime startDate = session.StartDate;
            DateTime endDate = startDate;
            float sessionCellHeight = THREE_LINES_CELL_MINIMUM_HEIGHT;
            if (session.EndDate != null)
            {
                endDate = (DateTime)session.EndDate;
                double sessionHoursDuration = endDate.Subtract(startDate).TotalHours;
                sessionCellHeight = 100 * (float)sessionHoursDuration;
            }

            float sessionTopSpace = 100 * (float)startDate.Subtract(new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0)).TotalHours;

            string cellFormStyle = $"height: {sessionCellHeight.ToString().Replace(",", ".")}%; top: {sessionTopSpace.ToString().Replace(",", ".")}%;";

            string cellHoursLineGridTemplateStyle = session.EndDate != null ? "grid-template-columns: 1fr 1fr 1fr; column-gap: 5%;" : "grid-template-columns: 1fr;";

            string startDateHourAndMinute = $"{(session.StartDate.Hour >= 10 ? "" : "0")}{session.StartDate.Hour}h{(session.StartDate.Minute >= 10 ? "" : "0")}{session.StartDate.Minute}";
            string endDateHourAndMinute = "";
            if (session.EndDate != null)
                endDateHourAndMinute = $"{(endDate.Hour >= 10 ? "" : "0")}{endDate.Hour}h{(endDate.Minute >= 10 ? "" : "0")}{endDate.Minute}";

            float bigCellFontSize = Math.Clamp(14f * sessionCellHeight / 100f, 14f, 20f);
            int patientFullNameSize = string.Concat(session.Patient.FirstName, " ", session.Patient.Name).Length;
            int sessionTitleSize = session.Title.Length;
            float patientNameFontSizeLengthFactor = patientFullNameSize <= 20f ? 1f : Math.Clamp(1f - 0.03f * (patientFullNameSize - 20f), 0.25f, 1f);
            float sessionTitleFontSizeLengthFactor = sessionTitleSize <= 40f ? 1f : Math.Clamp(1f - 0.03f * (sessionTitleSize - 40f), 0.5f, 1f);
            float smallCellFontSize = Math.Clamp(10f * sessionCellHeight / 100f, 10f, 14f);

            float bigCellHeight = sessionCellHeight < THREE_LINES_CELL_MINIMUM_HEIGHT ? 100f : Math.Clamp(30f - 10f * ((sessionCellHeight - THREE_LINES_CELL_MINIMUM_HEIGHT) / (300f - THREE_LINES_CELL_MINIMUM_HEIGHT)), 10f, 30f);
            float smallCellHeight = sessionCellHeight < THREE_LINES_CELL_MINIMUM_HEIGHT ? 0f : Math.Clamp(20f - 10f * ((sessionCellHeight - THREE_LINES_CELL_MINIMUM_HEIGHT) / (300f - THREE_LINES_CELL_MINIMUM_HEIGHT)), 5f, 20f);

            string bigCellStyle = $"height: {bigCellHeight.ToString().Replace(",", ".")}%;";
            string patientNameStyle = $"font-size: {(bigCellFontSize * patientNameFontSizeLengthFactor).ToString().Replace(",", ".")}px";
            string sessionTitleStyle = $"font-size: {(bigCellFontSize * sessionTitleFontSizeLengthFactor).ToString().Replace(",", ".")}px";
            string smallCellStyle = $"font-size: {smallCellFontSize.ToString().Replace(",", ".")}px";
            cellHoursLineGridTemplateStyle += $" height: {smallCellHeight.ToString().Replace(",", ".")}%";

            result.Add("CellFormStyle", cellFormStyle);
            result.Add("BigCellStyle", bigCellStyle);
            result.Add("PatientNameStyle", patientNameStyle);
            result.Add("SessionTitleStyle", sessionTitleStyle);
            result.Add("CellHoursLineGridTemplateStyle", cellHoursLineGridTemplateStyle);
            result.Add("SmallCellStyle", smallCellStyle);
            result.Add("StartDateHourAndMinute", startDateHourAndMinute);
            result.Add("EndDateHourAndMinute", endDateHourAndMinute);
            result.Add("SessionCellHeight", sessionCellHeight.ToString());

            return result;
        }

        public static string GetCurrentTimeLineStyle()
        {
            float currentTimeLineTopSpace = 100f * (float)DateTime.Now.Subtract(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)).TotalHours;
            return $"top: {currentTimeLineTopSpace.ToString().Replace(",", ".")}%;";
        }

        public static DateTime GetActualEndDateTime(DateTime startDate, DateTime? endDate)
        {
            return endDate == null ? startDate.AddHours(NO_END_DATE_FILLED_HOURS_COUNT) : (DateTime)endDate;
        }
    }
}
