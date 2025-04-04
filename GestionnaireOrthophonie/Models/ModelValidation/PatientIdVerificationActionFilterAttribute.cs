using GestionnaireOrthophonie.Data;
using GestionnaireOrthophonie.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GestionnaireOrthophonie.Models.ModelValidation
{
    public class PatientIdVerificationActionFilterAttribute : ActionFilterAttribute
    {
        private readonly DatabaseContext _databaseContext;

        public PatientIdVerificationActionFilterAttribute(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            int? patientId = context.ActionArguments["patientId"] as int?;

            if (patientId == null) return;

            Patient? patient = _databaseContext.Patients.FirstOrDefault(patient => patient.Id == patientId);

            if (patient == null)
            {
                context.ModelState.AddModelError("Patient", "The patient ID is not recognized by the database...");
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
            }
        }
    }
}
