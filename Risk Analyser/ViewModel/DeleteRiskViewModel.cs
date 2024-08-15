using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Repository;
using Risk_analyser.Model;
using Risk_analyser.MVVM;
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
        private RiskRepository riskRepository {  get; set; } 
        private Risk SelectedRisk { get; }
        public bool DialogResult { get; private set; }
        public string RiskWithExtraText {
            get { return SelectedRisk.Name + " ?"; }
             
        }
        public ICommand DeleteRiskCommand { get; set; }
        public ICommand CancelRiskCommand { get; set; }

        public DeleteRiskViewModel(DataContext context, Risk selectedRisk)
        {
            riskRepository=new RiskRepository(context);
            SelectedRisk = selectedRisk;
            DeleteRiskCommand=new RelayCommand(_=>DeleteRisk(),_=>CanDeleteRisk());
            CancelRiskCommand =new RelayCommand(_=>CloseWindow(),_=>true);
        }
        private void  DeleteRisk()
        {
            if (this.CanDeleteRisk())
            {
                riskRepository.DeleteRisk(SelectedRisk);
                System.Windows.MessageBox.Show("Ryzyko zostało pomyślnie usunięte", "Usuwanie Ryzyka",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Nie można usunąć ryzyka ponieważ dla zasobu do którego należy ryzyko ,zostały wygenerowane raporty z analiz." +
                                "Usuń wszystkie raporty które związane są  tym ryzykiem, aby móc go usunąć", "Usuwanie Ryzyka",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            DialogResult = true;
            CloseWindow();
        }
        private bool CanDeleteRisk()
        {
            return riskRepository.CanDeleteRisk(SelectedRisk);
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
