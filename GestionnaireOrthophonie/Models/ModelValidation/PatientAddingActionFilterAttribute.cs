using GestionnaireOrthophonie.Data;
using GestionnaireOrthophonie.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GestionnaireOrthophonie.Models.ModelValidation
{
    public class PatientAddingActionFilterAttribute : ActionFilterAttribute
    {
        private readonly DatabaseContext _databaseContext;

        public PatientAddingActionFilterAttribute(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            Patient? patient = context.ActionArguments["patient"] as Patient;

            if (patient == null)
                return;

            Patient[] databaseIdenticalPatients = _databaseContext.Patients
                .Where(p => p.FirstName == patient.FirstName && p.Name == patient.Name && p.Age == patient.Age && p.Gender == patient.Gender).ToArray();

            if (databaseIdenticalPatients.Length > 0)
            {
                context.ModelState.AddModelError("Patient", "Ce patient existe déjà dans la base de données...");
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
            }
        }
    }
}
