using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public enum TypeRiskValues
    {
        [Display(Name = "Integralność")]
        INT,
        [Display(Name = "Poufność")]
        CON,
        [Display(Name = "Dostępność")]
        AVA
    }
    public class RiskType
    {
        public long RiskTypeId { get; set; }
        public long RiskId { get; set; }
        public Risk Risk { get; set; }
        public TypeRiskValues Type { get; set; }
    }
}
