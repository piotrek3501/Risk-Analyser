using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Model.Entities
{
    public class Rhombus
    {
        public enum TypeInovation
        {

            [Description("Pochodna")]
            Derivative=1,
            [Description("Platformowy")]
            Platform=2,
            [Description("Przełomowy")]
            Crucial=3
        }
        public enum TypeTechnology
        {
            [Description("Nisko zaawansowana")]
            Low=1,
            [Description("Średnio zaawansowana")]
            Medium=2,
            [Description("Wysoko zaawansowana")]
            High=3,
            [Description("Bardzo wysoko zaawansowana")]
            VeryHigh=4
        }
        public enum TypeComplexity
        {
            [Description("Montażowa")]
            Assembly=1,
            [Description("Systemowa")]
            Systemic=2,
            [Description("Macierzowa")]
            Matrix=3
        }
        public enum TypeRate
        {
            [Description("Zwykłe")]
            Normal=1,
            [Description("Szybkie")]
            Fast=2,
            [Description("Błyskawiczne")]
            Instant=3
        }
        public long RhombusId { get; set; }

        [Required]
        [Display(Name = "Innowacyjność")]
        public TypeInovation Inovation { get; set; }

        [Required]
        [Display(Name = "Zaawansowanie technologiczne")]
        public TypeTechnology Technology { get; set; }

        [Required]
        [Display(Name = "Złożoność")]
        public TypeComplexity Complexity { get; set; }

        [Required]
        [Display(Name = "Tempo wdrażania")]
        public TypeRate Rate { get; set; }

        [Required]
        [Display(Name = "Ryzyko")]
        public Risk Risk { get; set; }

        public long? RiskId { get; set; }


    }
}
