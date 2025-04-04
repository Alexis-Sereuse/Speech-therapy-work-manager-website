using GestionnaireOrthophonie.Data;
using GestionnaireOrthophonie.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionnaireOrthophonie.Models
{
    public class PlanningOperationsManager
    {
        private readonly DatabaseContext _databaseContext;

        private const string DEFAULT_PERIOD_TYPE_NAME = "Semaine";

        public PlanningOperationsManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        #region PERIODS MANAGEMENT

        public void SetCurrentPeriodType(string periodTypeName)
        {
            NamedPeriod? wantedPeriodType = GetPeriodTypeByName(periodTypeName);
            if (wantedPeriodType == null) return;
            _databaseContext.PlanningOptions.First().VisualizationPeriodId = wantedPeriodType.Id;
            _databaseContext.PlanningOptions.First().VisualizationPeriod = wantedPeriodType;
            _databaseContext.SaveChanges();
        }

        public List<NamedPeriod> GetAllPeriodTypes()
        {
            return _databaseContext.NamedPeriods.ToList();
        }

        public NamedPeriod? GetCurrentPeriodType(string userId)
        {
            PlanningOptions? planningOptions = null;

            if (_databaseContext.PlanningOptions.Count() == 0)
            {
                planningOptions = AddUserPlanningOptions(userId);
            }
            else
            {
                planningOptions = _databaseContext.PlanningOptions.FirstOrDefault(options => options.UserId == userId);

                if (planningOptions == null)
                    planningOptions = AddUserPlanningOptions(userId);
            }

            _databaseContext.PlanningOptions.Entry(planningOptions).Navigation("VisualizationPeriod").Load();
            return planningOptions.VisualizationPeriod;
        }

        private PlanningOptions? AddUserPlanningOptions(string userId)
        {
            NamedPeriod? period = GetPeriodTypeByName(DEFAULT_PERIOD_TYPE_NAME);

            if (period == null)
                return null;

            PlanningOptions planningOptions = new PlanningOptions(userId, period.Id, period);
            _databaseContext.PlanningOptions.Add(planningOptions);
            _databaseContext.SaveChanges();

            return planningOptions;
        }

        private NamedPeriod? GetPeriodTypeByName(string periodTypeName)
        {
            return _databaseContext.NamedPeriods.FirstOrDefault(period => period.Name == periodTypeName);
        }

        #endregion

        #region SESSIONS MANAGEMENT

        private List<Session> GetVisualizedPeriodSessions(string userId)
        {
            if (PeriodsCalculationManager.CurrentPeriodBorders.Length == 0 && PeriodsCalculationManager.VisualizedPeriodBorders.Length == 0)
                PeriodsCalculationManager.SetCurrentAndVisualizedPeriods(GetCurrentPeriodType(userId));

            return _databaseContext.Sessions.Where(session => session.UserId == userId && session.StartDate >= PeriodsCalculationManager.VisualizedPeriodBorders[0] && session.StartDate <= PeriodsCalculationManager.VisualizedPeriodBorders[1]).Include(session => session.Patient).ToList();
        }

        public Dictionary<int, List<Session>> GetVisualizedPeriodSessionsByDay(string userId)
        {
            Dictionary<int, List<Session>> sessionsByDay = new Dictionary<int, List<Session>>();
            List<Session> visualizedPeriodSessions = GetVisualizedPeriodSessions(userId);
            NamedPeriod? currentPeriodType = GetCurrentPeriodType(userId);

            if (currentPeriodType == null)
                return sessionsByDay;

            int periodDaysCount = currentPeriodType.WeeksLength * 7;
            for (int i = 0; i < periodDaysCount; i++)
            {
                List<Session> daySessions = visualizedPeriodSessions.Where(session => session.StartDate.Day == PeriodsCalculationManager.VisualizedPeriodBorders[0].AddDays(i).Day).ToList();
                sessionsByDay.Add(i, daySessions);
            }

            return sessionsByDay;
        }

        public void AddSession(Session sessionToAdd)
        {
            _databaseContext.Sessions.Add(sessionToAdd);
            _databaseContext.SaveChanges();
        }

        public Session? GetSessionById(int sessionId)
        {
            return _databaseContext.Sessions.Include(session => session.Patient).FirstOrDefault(session => session.Id == sessionId);
        }

        public void ModifySession(Session modifiedSession)
        {
            Session? sessionToModify = _databaseContext.Sessions.FirstOrDefault(session => session.Id == modifiedSession.Id);

            if (sessionToModify == null)
                return;

            sessionToModify.StartDate = modifiedSession.StartDate;
            sessionToModify.EndDate = modifiedSession.EndDate;
            sessionToModify.Title = modifiedSession.Title;
            sessionToModify.Patient = modifiedSession.Patient;
            sessionToModify.PreparationInformation = modifiedSession.PreparationInformation;
            sessionToModify.ProgressInformation = modifiedSession.ProgressInformation;
            sessionToModify.Patient = modifiedSession.Patient;
            sessionToModify.PatientId = modifiedSession.PatientId;

            _databaseContext.SaveChanges();
        }

        public void DeleteSession(int sessionId)
        {
            Session? sessionToRemove = _databaseContext.Sessions.FirstOrDefault(session => session.Id == sessionId);

            if (sessionToRemove == null)
                return;

            _databaseContext.Sessions.Remove(sessionToRemove);
            _databaseContext.SaveChanges();
        }

        #endregion
    }
}
