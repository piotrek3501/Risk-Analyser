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
    public class RiskService
    {
        private RiskRepository _RiskRepository;
        private DataContext _context;
        public RiskService(DataContext context)
        {
            _context = context;
            _RiskRepository=new RiskRepository(context);
        }
        public List<Risk> GetRisksFromSelection(List<RiskItem> SelectedRisks)
        {
            List<Risk> Risks = new List<Risk>();

            foreach (RiskItem r in SelectedRisks)
            {
                if (r.BelongToControlRisk == true)
                {
                    Risks.Add(_RiskRepository.GetRiskById(r.RiskId));
                }
            }
            return Risks;
        }

    }
}
