using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Risk_analyser.Data.Repository
{
    public class ControlRiskRepository
    {
        private  DbSet<ControlRisk> Controls;
        private  ContextManager ContextManager;
        public ControlRiskRepository(DbSet<ControlRisk> controls) {

            Controls= controls;
        }

        public ControlRisk LoadControlRiskWithEntities(long Id)
        {
            return Controls.Include(x => x.Risks)
                .Include(x => x.Mitagations)
                .Single(x => x.ControlRiskId == Id);
        }
        public void EditControlRisk(ControlRisk control)
        {
            Controls.Update(control);
            ContextManager.SaveChanges();
        }
        public void AddControlRisk(ControlRisk control)
        {
            Controls.Add(control);
            ContextManager.SaveChanges();
        }
        public void DeleteControlRisk(ControlRisk control)
        {
            Controls.Remove(control);
            ContextManager.SaveChanges();
        }
        public List<ControlRisk> GetAllControlRisksForRisk(Risk risk)
        {
            return Controls.Where(x => x.Risks.Contains(risk)).ToList();
        }

      
    }
}
