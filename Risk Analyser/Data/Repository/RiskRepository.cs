using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Repository
{
    public class RiskRepository
    {
        private DataContext _context;
        private DbSet<Risk> risks;
        private DbSet<Asset> assets;
        private DbSet<FRAPAnalysis> FRAPAnalysis;
        private DbSet<RhombusAnalysis> RhombusAnalyses;

        public RiskRepository(DataContext context)
        {
            _context = context;
            risks=_context.Risks;
            FRAPAnalysis =_context.FRAPAnalyses;
            RhombusAnalyses = _context.RhombusAnalyses;
        }
        public Risk LoadRiskWithEntities(long Id)
        {
            return risks.Include(x=>x.Asset)
                .Include(x=>x.Controls)
                .Include(x=>x.DoneMitigationActions)
                .Include(x=>x.RhombusParams)
                .Include(x=>x.Types)
                .Single(x => x.RiskId == Id);
        }
        public Risk GetRiskById(long id)
        {
            return risks.Where(x=>x.RiskId==id).Single();
        }
        public void EditRisk(Risk risk)
        {
            risks.Update(risk);
            _context.SaveChanges();
        }
        public void SaveRisk(Risk risk)
        {
            risks.Add(risk);
            _context.SaveChanges();
        }
        public void DeleteRisk(Risk risk)
        {
            risks.Remove(risk);
            _context.SaveChanges();
        }
        public bool CanDeleteRisk(Risk risk)
        {
            risk =LoadRiskWithEntities(risk.RiskId);

            bool anyFRAP=_context.FRAPAnalyses.Where(x => x.Asset.AssetId == risk.Asset.AssetId).Any();
            bool anyRhombus=_context.RhombusAnalyses.Where(x=>x.Asset.AssetId== risk.Asset.AssetId).Any();
            return !(anyFRAP || anyRhombus);
        }

    }
}
