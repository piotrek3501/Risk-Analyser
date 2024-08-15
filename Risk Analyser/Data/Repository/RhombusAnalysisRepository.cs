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
    public class RhombusAnalysisRepository
    {
        private DataContext _context;
        private DbSet<RhombusAnalysis> RhombusAnalyses;
        public RhombusAnalysisRepository(DataContext context)
        {
            _context = context;
            RhombusAnalyses = context.RhombusAnalyses;
        }
        public bool CanDeleteRisk(FRAPAnalysis analysis)
        {
            return true;
        }
    }
}
