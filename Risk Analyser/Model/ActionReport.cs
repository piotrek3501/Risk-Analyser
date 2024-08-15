using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public class ActionReport
    {
        public long ActionReportId { get; set; }

        [Required]
        [Display(Name ="Ryzyko")]
        public Risk Risk { get; set; }

        public long RiskId { get; set; }

        [Required]
        [Display(Name ="Podjęte działania")]
        public MitagationAction Action { get; set; }

        public long MitagatioActionId { get; set; }

       

    }

}
