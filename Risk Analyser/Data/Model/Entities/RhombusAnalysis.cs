﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Model.Entities
{
    public class RhombusAnalysis
    {
        public long RhombusAnalysisId { get; set; }

        [Required]
        [Display(Name = "Rezultat Analizy")]
        public List<RhombusDocument> Results { get; set; }

        [Required]
        [Display(Name = "Zasób")]
        public Asset Asset { get; set; }


    }
}
