using Risk_analyser.Data.DBContext;
using Risk_analyser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.ViewModel
{
    public class UpdateOrAddMitagationViewModel
    {
        public UpdateOrAddMitagationViewModel(DataContext context, MitagationAction selectedMitagation)
        {
            Context = context;
            SelectedMitagation = selectedMitagation;
        }

        public UpdateOrAddMitagationViewModel(DataContext context, ControlRisk selectedControlRisk)
        {
            Context = context;
            SelectedControlRisk = selectedControlRisk;
        }

        public DataContext Context { get; }
        public MitagationAction SelectedMitagation { get; }
        public ControlRisk SelectedControlRisk { get; }
        public bool DialogResult { get; internal set; }
    }
}
