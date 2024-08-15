using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public class Rhombus
    {
        public enum TypeInovation
        {
         
            [Description("Pochodna")]
            Derivative,
            [Description("Platformowy")]
            Platform,
            [Description("Przełomowy")]
            Crucial
        }
        public enum TypeTechnology
        {
            [Description("Nisko zaawansowana")]
            Low,
            [Description("Średnio zaawansowana")]
            Medium,
            [Description("Wysoko zaawansowana")]
            High,
            [Description("Bardzo wysoko zaawansowana")]
            VeryHigh
        }
        public enum TypeComplexity
        {
            [Description("Montażowa")]
            Assembly,
            [Description("Systemowa")]
            Systemic,
            [Description("Macierzowa")]
            Matrix
        }
        public enum TypeRate
        {
            [Description("Zwykłe")]
            Normal,
            [Description("Szybkie")]
            Fast,
            [Description("Błyskawiczne")]
            Instant
        }
        public long RhombusId { get; set; }

        [Required]
        [Display(Name ="Innowacyjność")]
        public TypeInovation Inovation { get; set; }

        [Required]
        [Display(Name ="Zaawansowanie technologiczne")]
        public TypeTechnology Technology { get; set; }

        [Required]
        [Display(Name ="Złożoność")]
        public TypeComplexity Complexity { get; set; }

        [Required]
        [Display(Name ="Tempo wdrażania")]
        public TypeRate Rate { get; set; }

        [Required]
        [Display(Name = "Ryzyko")]
        public Risk Risk { get; set; }

        public long? RiskId { get; set; }

        
    }
}
