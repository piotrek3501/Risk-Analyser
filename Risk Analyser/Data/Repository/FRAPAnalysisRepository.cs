using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Repository
{
    public class FRAPAnalysisRepository
    {
        private DataContext _context;
        private DbSet<FRAPAnalysis> FRAPAnalyses;
        public FRAPAnalysisRepository(DataContext context) {
            _context = context;
            FRAPAnalyses=context.FRAPAnalyses;
        }
        public bool CanDeleteRisk(FRAPAnalysis analysis)
        {
            return true;
        }
    }
}
