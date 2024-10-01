using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using Risk_analyser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class DeleteControlRiskViewModel:ViewModelBase
    {
        private ControlRisk _control { get; set; }
        private Risk SelectedRisk { get; set; }
        private ControlRiskService ControlService { get; set; }
        public ICommand DeleteControlCommand { get; }
        public ICommand CancelControlCommand { get; }
        public bool DialogResult { get; private set; }
        public string ControlWithExtraText
        {
            get
            {
                return Control.Name + " ?";
            }

        }
        public ControlRisk Control
        {
            get
            {
                return _control;
            }
            set
            {
                _control = value;
                OnPropertyChanged();
            }
        }

       

        public DeleteControlRiskViewModel(ControlRisk control, Risk selectedRisk)
        {
            SelectedRisk = selectedRisk;
            ControlService = MainWindowService.GetControlRiskService();
            Control = ControlService.LoadControlRiskWithEntities(control.ControlRiskId);
            DeleteControlCommand = new RelayCommand(_ => DeleteControl(), _ => true);
            CancelControlCommand = new RelayCommand(_ => CloseWindow(), _ => true);
        }
       

        private void DeleteControl()
        {
            ControlService.DeleteControlRisk(Control,SelectedRisk);
            DialogResult = true;
            CloseWindow();

        }

        private void CloseWindow()
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = DialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}
