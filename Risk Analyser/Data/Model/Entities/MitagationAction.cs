using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Model.Entities
{
    public class MitagationAction
    {
        public long MitagatioActionId { get; set; }

        [Required]
        [Display(Name = "Pracownik")]
        public string Person { get; set; }

        [Required]
        [Display(Name = "Data wykonania akcji")]
        public DateTime DateOfAction { get; set; }

        [Required]
        [Display(Name ="Czy wykonano ?")]
        public bool Status {  get; set; }

        [Required]
        [Display(Name = "Wykonana czynność")]
        public string Action { get; set; }

        [Required]
        [Display(Name = "Kategorie")]
        public List<ControlRisk> ControlRisks { get; set; }

        [Required]
        [Display(Name ="Data utworzenia akcji")]
        public DateTime CreationTime { get; set; }

    }
}
