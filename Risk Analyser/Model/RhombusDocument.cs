﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Model
{
    public class RhombusDocument:Document
    {
        public RhombusAnalysis? RhombusAnalysis { get; set; }
        public long? RhombusAnalysisId { get; set; }

    }
}
