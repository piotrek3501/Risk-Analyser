using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Repository;
using Risk_analyser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.services
{
    public  class ControlRiskService
    {
        private DataContext _context;
        private RiskService RiskService;
        private MitigationActionService MitigationActionService;
        public ControlRiskService(DataContext context) { 
            _context=context;
            RiskService=new RiskService(context);
            MitigationActionService=new MitigationActionService(context);
        } 
        public List<Risk> GetSelectedRisks(List<RiskItem>selectedRisks)
        {
            return RiskService.GetRisksFromSelection(selectedRisks);
        }
        public List<MitagationAction> GetSelectedMitigation(List<MitagationItem> selectedMitigation)
        {
            return MitigationActionService.GetMitgationFromSelection(selectedMitigation);
        }
    }
}
