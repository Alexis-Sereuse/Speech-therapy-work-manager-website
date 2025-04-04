using GestionnaireOrthophonie.Data;
using GestionnaireOrthophonie.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace GestionnaireOrthophonie.Models.ModelValidation
{
    public class SessionDatesValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DatabaseContext? databaseContext = validationContext.GetService<DatabaseContext>() as DatabaseContext;

            Session? currentSession = validationContext.ObjectInstance as Session;

            if (currentSession != null)
            {
                List<string> errorMessages = new List<string>();

                if (currentSession.EndDate != null)
                {
                    if (currentSession.StartDate.CompareTo(currentSession.EndDate) > 0)
                        errorMessages.Add("L'horaire de fin ne peut pas se trouver plus tôt que l'horaire de début.");

                    if (currentSession.StartDate.Day != ((DateTime)currentSession.EndDate).Day)
                        errorMessages.Add("L'horaire de début et l'horaire de fin doivent se trouver le même jour.");
                }

                if (databaseContext != null)
                {
                    DateTime actualEndDateTime = PeriodsCalculationManager.GetActualEndDateTime(currentSession.StartDate, currentSession.EndDate);

                    List<Session> sessionsContainingStartDateTime = databaseContext.Sessions.Where(session => session.Id != currentSession.Id && session.StartDate <= currentSession.StartDate).ToList();
                    sessionsContainingStartDateTime = sessionsContainingStartDateTime.Where(session => (session.EndDate == null ?
                        session.StartDate.AddHours(PeriodsCalculationManager.NO_END_DATE_FILLED_HOURS_COUNT) : (DateTime)session.EndDate) > currentSession.StartDate).ToList();

                    List<Session> sessionsContainingEndDateTime = databaseContext.Sessions.Where(session => session.Id != currentSession.Id && session.StartDate < actualEndDateTime).ToList();
                    sessionsContainingEndDateTime = sessionsContainingEndDateTime.Where(session => (session.EndDate == null ?
                        session.StartDate.AddHours(PeriodsCalculationManager.NO_END_DATE_FILLED_HOURS_COUNT) : (DateTime)session.EndDate) >= actualEndDateTime).ToList();

                    List<Session> sesssessionsContainedInCurrentSession = databaseContext.Sessions.Where(session => session.Id != currentSession.Id && session.StartDate >= currentSession.StartDate).ToList();
                    sesssessionsContainedInCurrentSession = sesssessionsContainedInCurrentSession.Where(session => (session.EndDate == null ?
                        session.StartDate.AddHours(PeriodsCalculationManager.NO_END_DATE_FILLED_HOURS_COUNT) : (DateTime)session.EndDate) <= actualEndDateTime).ToList();

                    bool startDateIsCorrect = sessionsContainingStartDateTime.Count == 0;
                    bool endDateIsCorrect = sessionsContainingEndDateTime.Count == 0;
                    bool sessionDurationIsCorrect = sesssessionsContainedInCurrentSession.Count == 0;

                    if (!sessionDurationIsCorrect)
                        errorMessages.Add("La plage horaire renseignée contient au moins une séance déjà existante.");

                    if (!startDateIsCorrect && !endDateIsCorrect)
                        errorMessages.Add("L'horaire de début et l'horaire de fin sont contenus dans une ou deux autres séances.");
                    else if (!startDateIsCorrect)
                        errorMessages.Add("L'horaire de début se trouve dans la plage horaire d'une séance déjà existante.");
                    else if (!endDateIsCorrect)
                        errorMessages.Add("L'horaire de fin se trouve dans la plage horaire d'une séance déjà existante.");
                }

                if (errorMessages.Count > 0)
                {
                    string error = "";
                    foreach (string errorMessage in errorMessages)
                        error += errorMessage + "\n";
                    error.Remove(error.Length - 2);
                    return new ValidationResult(error);
                }
            }

            return ValidationResult.Success;
        }
    }
}
