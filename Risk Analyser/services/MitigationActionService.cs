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
    public class MitigationActionService
    {
        private MitagationRepository _MitagationRepository;
        private DataContext _context;
        public MitigationActionService(DataContext context) { 
            _context = context;
            _MitagationRepository=new MitagationRepository(context);
        }
        public List<MitagationAction> GetMitgationFromSelection(List<MitagationItem> SelectedMitagation)
        {
            List<MitagationAction> mitagationActions = new List<MitagationAction>();
            foreach (MitagationItem m in SelectedMitagation)
            {
                if (m.BelongToControlRisk == true)
                {
                    mitagationActions.Add(_MitagationRepository.GetMitagationFromId(m.IdMitagation));
                }
            }
            return mitagationActions;
        }


    }
}
