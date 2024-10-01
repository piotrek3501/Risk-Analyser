using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
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
    public class DeleteRiskViewModel:ViewModelBase
    {
        private RiskService RiskService { get; set; }
        private Risk SelectedRisk { get; }
        public bool DialogResult { get; private set; }
        public string RiskWithExtraText {
            get { return SelectedRisk.Name + " ?"; }
             
        }
        public ICommand DeleteRiskCommand { get; set; }
        public ICommand CancelRiskCommand { get; set; }

        public DeleteRiskViewModel(Risk selectedRisk)
        {
            RiskService = MainWindowService.GetRiskService();
            SelectedRisk = selectedRisk;
            DeleteRiskCommand=new RelayCommand(_=>DeleteRisk(),_=>true);
            CancelRiskCommand =new RelayCommand(_=>CloseWindow(),_=>true);
        }
        private void  DeleteRisk()
        {
            RiskService.DeleteRisk(SelectedRisk);
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
