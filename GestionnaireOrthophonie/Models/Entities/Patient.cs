using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireOrthophonie.Models.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string? Gender { get; set; }
        public string? Anamnesis { get; set; }
        public string? Progress { get; set; }
        public string? Difficulties { get; set; }
        public string? Pathologies { get; set; }
        public string? TherapeuticObjectives { get; set; }

        List<Session>? Sessions { get; set; }
    }
}
