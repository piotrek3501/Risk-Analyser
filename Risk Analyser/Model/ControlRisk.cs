using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public class ControlRisk
    {
        public long ControlRiskId { get; set; }

        [Required]
        [Display(Name="Nazwa")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Opis")]
        public string Description { get; set; }

        [Display(Name="Powiązane Ryzyka")]
        public List<Risk>? Risks { get; set; }

        [Display(Name = "Akcje łagodzące ryzyko")]
        public List<MitagationAction>? Mitagations { get; set; }
    }
}
