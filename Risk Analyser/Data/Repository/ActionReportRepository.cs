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
    public class ActionReportRepository
    {
        private DataContext _context;
        private DbSet<ActionReport> ActionReports;
        public ActionReportRepository(DataContext context)
        {
            _context = context;
            ActionReports = _context.ActionReports;
        }

        public ActionReport LoadActionReportkWithEntities(long Id)
        {
            return ActionReports.Include(x => x.Risk)
                .Include(x => x.Action)
                .Single(x => x.ActionReportId == Id);
        }
        public void EditRisk(ActionReport actionReport)
        {
            ActionReports.Update(actionReport);
            _context.SaveChanges();
        }
        public void SaveRisk(ActionReport actionReport)
        {
            ActionReports.Add(actionReport);
            _context.SaveChanges();
        }
        public void DeleteRisk(ActionReport actionReport)
        {
            ActionReports.Remove(actionReport);
            _context.SaveChanges();
        }
        public bool CanDeleteRisk(ControlRisk control)
        {
            return true;
        }
        
    }
}
