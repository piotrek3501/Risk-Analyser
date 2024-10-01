using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data
{
    public class ContextManager
    {
        private static  readonly DataContext _context=new DataContext();

        public DbSet<Asset> Assets => _context.Assets;
        public DbSet<ControlRisk> ControlRisks => _context.ControlsRisks;
        public DbSet<FRAPAnalysis> FrapAnalysis => _context.FRAPAnalyses;
        public DbSet<RhombusAnalysis> RhombusAnalyses => _context.RhombusAnalyses;
        public DbSet<Risk> Risks => _context.Risks;
        public DbSet<MitagationAction> mitagationActions => _context.MitagationActions;
        public DbSet<FRAPDocument> FRAPDocuments => _context.FRAPDocuments;
        public DbSet<RhombusDocument> rhombusDocuments=> _context.RhombusDocuments;

        public ContextManager()
        {           
        }

        public static void SaveChanges()
        {
            _context.SaveChanges();
            
        }

    }
}
