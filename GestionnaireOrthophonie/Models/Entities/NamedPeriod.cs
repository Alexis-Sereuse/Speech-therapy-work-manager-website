using System.ComponentModel.DataAnnotations;

namespace GestionnaireOrthophonie.Models.Entities
{
    public class NamedPeriod
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public int WeeksLength { get; set; }
    }
}
