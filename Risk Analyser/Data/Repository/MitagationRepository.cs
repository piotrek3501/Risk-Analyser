using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Model;
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
        private DataContext _context;
        private DbSet<ControlRisk> Controls;
        private DbSet<MitagationAction> MitagationActions;
        private DbSet<ActionReport> ActionReports;
        private ControlRiskRepository ControlRiskRepository;
        private ActionReportRepository ActionReportRepository;
        public MitagationRepository(DataContext context)
        {
            _context = context;
            Controls = _context.ControlsRisks;
            ActionReports = _context.ActionReports;
            ControlRiskRepository=new ControlRiskRepository(context);
            ActionReportRepository=new ActionReportRepository(context);
        }

        public MitagationAction LoadMitagationWithEntities(long Id)
        {
            return MitagationActions.Include(x => x.ControlRisks)
                .Include(x => x.ActionReports)
                .Single(x => x.MitagatioActionId == Id);
        }
        public void EditRisk(MitagationAction mitgation)
        {
            MitagationActions.Update(mitgation);
            _context.SaveChanges();
        }
        public void SaveRisk(MitagationAction mitgation)
        {
            MitagationActions.Add(mitgation);
            _context.SaveChanges();
        }
        public void DeleteRisk(MitagationAction mitgation)
        {
            MitagationActions.Remove(mitgation);
            _context.SaveChanges();
        }
        public bool CanDeleteMitigation(MitagationAction mitgation)
        {
            mitgation = LoadMitagationWithEntities(mitgation.MitagatioActionId);

            Risk relatedRisk = null;
            int initVar = 1;
            foreach(ActionReport r in mitgation.ActionReports)
            {
                ActionReport rep= ActionReportRepository.LoadActionReportkWithEntities(r.ActionReportId);
                if(initVar == 1)
                {
                    relatedRisk=rep.Risk;
                    initVar++;
                }
                if (rep.Risk.RiskId != relatedRisk.RiskId)
                {
                    bool AnyOtherDiffrentRelatedActionReports = true;
                    return !AnyOtherDiffrentRelatedActionReports;
                }
            }
            List<ControlRisk> ControlRiskList = mitgation.ControlRisks.ToList();
            bool anyOtherDiffrentRelatedControlRisk;
            foreach (ControlRisk controlRisk in ControlRiskList)
            {
                ControlRisk control=ControlRiskRepository.LoadControlRiskWithEntities(controlRisk.ControlRiskId);
                List<Risk>riskFromControl=control.Risks;
                foreach(Risk r in riskFromControl)
                {
                    if (r.RiskId != relatedRisk.RiskId)
                    {
                        anyOtherDiffrentRelatedControlRisk = true;
                        return !anyOtherDiffrentRelatedControlRisk;
                    }
                }   
            }  
            return true;
        }
        public MitagationAction GetMitagationFromId(long id)
        {
            return MitagationActions.Where(x => x.MitagatioActionId == id).Single();
        }

    }
}
