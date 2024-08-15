using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Model;
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
        private DataContext _context;
        private DbSet<ControlRisk> Controls;
        private DbSet<MitagationAction> MitagationActions;
        private DbSet<Risk> Risks;
        public ControlRiskRepository(DataContext context) {
            _context = context;
            Controls=_context.ControlsRisks;
        }

        public ControlRisk LoadControlRiskWithEntities(long Id)
        {
            return Controls.Include(x => x.Risks)
                .Include(x => x.Mitagations)
                .Single(x => x.ControlRiskId == Id);
        }
        public void EditRisk(ControlRisk control)
        {
            Controls.Update(control);
            _context.SaveChanges();
        }
        public void SaveRisk(ControlRisk control)
        {
            Controls.Add(control);
            _context.SaveChanges();
        }
        public void DeleteRisk(ControlRisk control)
        {
            Controls.Remove(control);
            _context.SaveChanges();
        }
        public bool CanDeleteRisk(ControlRisk control)
        {
            control = LoadControlRiskWithEntities(control.ControlRiskId);

            bool anyMitagations = _context.MitagationActions.Where(x => x.ControlRisks.Contains(control)).Any();
            var Risks = _context.Risks.Where(x => x.Controls.Contains(control));
            bool AnyOtherRisks;
            if(Risks.Count()==1 && Risks.Last().Controls.Contains(control))
            {
                AnyOtherRisks = false;
            }
            else
            {
                AnyOtherRisks = true;
            }
            return !(anyMitagations || AnyOtherRisks);
        } 
        public List<MitagationAction> GetAllMitagationActions()
        {
            return new List<MitagationAction>(_context.MitagationActions);
        }
        public List<Risk> GetAllRiskWithAsset()
        {
            return new List<Risk>(_context.Risks.Include(x=>x.Asset));
        }
    }
}
