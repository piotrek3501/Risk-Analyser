using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Model.Entities
{

    public enum Priority
    {
        [Description("A")]
        A,
        [Description("B")]
        B,
        [Description("C")]
        C,
        [Description("D")]
        D
    }
    public enum Vulnerability
    {
        [Description("Wysoka")]
        High,
        [Description("Średnia")]
        Medium,
        [Description("Niska")]
        Low
    }
    public enum Propability
    {
        [Description("Niskie")]
        Low,
        [Description("Średnie")]
        Medium,
        [Description("Wysokie")]
        High
    }
    public enum PotencialEffect
    {
        [Description("Niskie")]
        Low,
        [Description("Średnie")]
        Medium,
        [Description("Wysokie")]
        High
    }
    public class Risk
    {
        public long RiskId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Zasób")]
        public Asset Asset { get; set; }

        public long AssetId { get; set; }

        [Required]
        [Display(Name = "Typ Ryzyka")]
        public List<RiskType> Types { get; set; }

        [StringLength(300), MinLength(3)]
        [Display(Name = "Opis")]
        public string? Description { get; set; }



        [Required]
        [Display(Name = "Priorytet")]
        public Priority Priority { get; set; }


        [Required]
        [Display(Name = "Podatność")]
        public Vulnerability Vulnerability { get; set; }



        [Display(Name = "Czynniki łagodzące")]
        public List<ControlRisk>? Controls { get; set; }

        [Required]
        [Display(Name = "Prawdopobieństwo")]
        public Propability Propability { get; set; }

        [Required]
        [Display(Name = "Potencjalne wpływ")]
        public PotencialEffect PotencialEffect { get; set; }

        [Required]
        public Rhombus RhombusParams { get; set; }

        [Required]

        public DateTime CreationTime { get; set; }


    }
}
