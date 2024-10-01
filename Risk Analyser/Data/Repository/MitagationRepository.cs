using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Risk_analyser.Data.Repository
{
    public class MitagationRepository
    {
        private  DbSet<MitagationAction> MitagationActions;
        private  ContextManager ContextManager;

        public MitagationRepository(DbSet<MitagationAction> mitagationActions)
        {
            MitagationActions = mitagationActions;
        }

        public MitagationAction LoadMitagationWithEntities(long Id)
        {
            return MitagationActions.Include(x => x.ControlRisks)
                .Single(x => x.MitagatioActionId == Id);
        }
        public void EditMitagationAction(MitagationAction mitgation)
        {
            MitagationActions.Update(mitgation);
            ContextManager.SaveChanges();
        }
        public void AddMitagationAction(MitagationAction mitgation)
        {
            MitagationActions.Add(mitgation);
            ContextManager.SaveChanges();
        }
        public void DeleteMitagationAction(MitagationAction mitgation)
        {
            MitagationActions.Remove(mitgation);
            ContextManager.SaveChanges();
        }
       
        public MitagationAction GetMitagationFromId(long id)
        {
            return MitagationActions.Where(x => x.MitagatioActionId == id).Single();
        }
        public List<MitagationAction> GetAllMitigationFromControlRisk(ControlRisk controlRisk)
        {
            return MitagationActions.Where(x=>x.ControlRisks.Any(y=>y.ControlRiskId==controlRisk.ControlRiskId)).ToList();
            //return MitagationActions.ToList();

        }
        public List<MitagationAction> GetAllMitigation()
        {
            return MitagationActions.ToList();
        }

    }
}
