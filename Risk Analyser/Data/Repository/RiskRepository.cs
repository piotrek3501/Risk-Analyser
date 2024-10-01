using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Repository
{
    public class RiskRepository
    {
        private readonly DbSet<Risk> risks;
        private readonly ContextManager ContextManager;


        public RiskRepository(DbSet<Risk> Risks)
        {
            risks= Risks;
        }
        public Risk LoadRiskWithEntities(long Id)
        {
            return risks.Include(x=>x.Asset)
                .Include(x=>x.Controls)
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
            ContextManager.SaveChanges();
        }
        public void AddRisk(Risk risk)
        {
            risks.Add(risk);
            ContextManager.SaveChanges();
        }
        public void DeleteRisk(Risk risk)
        {
            risks.Remove(risk);
            ContextManager.SaveChanges();
        }
        
        public List<Risk> GetRisksForAsset(Asset asset)
        {
            return risks.Where(x => x.Asset.AssetId == asset.AssetId).ToList();
        }
        public List<Risk> GetAllRiskWithAsset()
        {
            return risks.Include(x => x.Asset).ToList();
        }
        public List<Risk> GetRisksForControlRisk(long ControlId)
        {
                 return risks.Include(x => x.Controls).Include(y=>y.Asset).ToList().Where(c=>c.Controls.Exists(y=>y.ControlRiskId==ControlId)).ToList();
        }

    }
}
