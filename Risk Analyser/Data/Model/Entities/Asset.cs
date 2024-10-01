using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Model.Entities
{
    public class Asset
    {
        public long AssetId { get; set; }

        [Required]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Display(Name = "Ryzyka")]
        public List<Risk>? Risks { get; set; }

        [Display(Name = "Analizy FRAP")]
        public FRAPAnalysis FRAPAnalysis { get; set; }
        public long FRAPAnalysisId {  get; set; } 

        [Display(Name = "Analiza Romboidalna")]
        public RhombusAnalysis RhombusAnalysis { get; set; }
        public long RhombusAnalysisId { get; set; }


        [Display(Name = "Czas utworzenia")]
        public DateTime CreationTime { get; set; }

    }
}
