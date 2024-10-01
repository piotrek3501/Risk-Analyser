using Risk_analyser.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Risk_analyser.Data.Model
{
    public class MitagationDataGridItem:ViewModelBase
    {
        public string Worker { get; set; }
        public string Action { get; set; }
        public DateTime DateOfAction { get; set; }
        public long IdMitagation { get; set; }
        public bool BelongToControlRisk { get; set; }
        public bool Status { get; set; }
        private ICommand _deleteAction;
        public ICommand DeleteAction
        {
            get => _deleteAction;
            set
            {
                _deleteAction = value;
                OnPropertyChanged(nameof(DeleteAction));
            }
        }

        private ICommand _editAction;
        public ICommand EditAction
        {
            get => _editAction;
            set
            {
                _editAction = value;
                OnPropertyChanged(nameof(EditAction));
            }
        }
        public MitagationDataGridItem(long id, string action, string worker, DateTime date, bool belongToControl,bool status)
        {
            IdMitagation = id;
            Worker = worker;
            Action = action;
            DateOfAction = date;
            BelongToControlRisk = belongToControl;
            Status = status;

        }
    }

}
