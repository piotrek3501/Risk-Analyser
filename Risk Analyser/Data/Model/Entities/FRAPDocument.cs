﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Model.Entities
{
    public class FRAPDocument : Document
    {
        public FRAPAnalysis? FRAPAnalysis { get; set; }
        public long? FRAPAnalysisId { get; set; }
    }
}
