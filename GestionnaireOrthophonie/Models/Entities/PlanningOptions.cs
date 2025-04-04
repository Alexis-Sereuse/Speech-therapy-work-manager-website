using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionnaireOrthophonie.Models.Entities
{
    public class PlanningOptions
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("VisualizationPeriod")]
        public int VisualizationPeriodId { get; set; }
        [Required]
        public NamedPeriod? VisualizationPeriod { get; set; }

        public PlanningOptions() { }

        public PlanningOptions(string userId, int visualizationPeriodId, NamedPeriod? visualizationPeriod)
        {
            UserId = userId;
            VisualizationPeriodId = visualizationPeriodId;
            VisualizationPeriod = visualizationPeriod;
        }
    }
}
