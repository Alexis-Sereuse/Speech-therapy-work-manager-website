using GestionnaireOrthophonie.Models.ModelValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireOrthophonie.Models.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required] [SessionDatesValidation] public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required] public string? Title { get; set; }
        public string? PreparationInformation { get; set; }
        public string? ProgressInformation { get; set; }
        [ForeignKey("Patient")] [Required] public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public Session() { }

        public Session(int id, DateTime startDate, DateTime? endDate, string? title, string? preparationInformation, string? progressInformation, int patientId)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            Title = title;
            PreparationInformation = preparationInformation;
            ProgressInformation = progressInformation;
            PatientId = patientId;
        }
    }
}
