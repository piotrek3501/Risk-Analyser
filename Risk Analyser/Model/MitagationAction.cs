using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public class MitagationAction
    {
        public long MitagatioActionId { get; set; }

        [Required]
        [Display(Name = "Pracownik")]
        public string Person { get; set; }

        [Required]
        [Display(Name = "Data akcji")]
        public DateOnly DateOfAction { get; set; }

        [Required]
        [Display(Name ="Wykonana czynność")]
        public string Action { get; set; }

        [Required]
        [Display(Name ="Podjęte akcje")]
        public List<ActionReport> ActionReports { get; set; }

        [Required]
        [Display(Name ="Kategorie")]
        public List<ControlRisk>ControlRisks { get; set; }


    }
}
