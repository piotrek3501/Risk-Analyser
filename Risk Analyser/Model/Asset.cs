using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public class Asset
    {
        public long AssetId { get; set; }

        [Required]
        [Display(Name="Nazwa")]
        public string Name { get; set; }

        [Display(Name="Opis")]
        public string? Description { get; set; }

        [Display(Name = "Ryzyka")]
        public List<Risk>? Risks { get; set; }

        [Display(Name = "Analizy FRAP")]
        public List<FRAPAnalysis>? FRAPAnalysis { get; set; }

        [Display(Name = "Analiza Romboidalna")]
        public List<RhombusAnalysis>? RhombusAnalysis { get;set; }

        [Display(Name="Data utworzenia")]
        public DateTime CreationTime { get; set; }
        
    }
}
