using Risk_analyser.Model;
using Risk_analyser.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Input;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Repository;

namespace Risk_analyser.ViewModel
{
    public class DeleteAssetViewModel:ViewModelBase
    {
        private readonly DataContext _context;
        private Asset _asset { get; set; }
        private AssetRepository _assetRepository { get; set; }
        public ICommand DeleteAssetCommand { get;  }
        public ICommand CancelAssetCommand { get; }
        public bool DialogResult { get; private set; }
        public string AssetWithExtraText { 
            get {
                return Asset.Name + " ?";
            }
           
        }
        private Asset Asset { 
            get {
                return _asset;
            }
            set {
                _asset = value;
                OnPropertyChanged();
            }
        }

        public DeleteAssetViewModel(DataContext context,Asset asset) { 
        
            _context = context;
            _asset=asset;
            _assetRepository=new AssetRepository(context);
            DeleteAssetCommand = new RelayCommand(_=>DeleteAsset(),_=>Asset!=null && CanDeleteAsset());
            CancelAssetCommand = new RelayCommand(_=>CloseWindow(),_=>true);
        }

        private void DeleteAsset()
        {
            if (CanDeleteAsset())
            {
                _assetRepository.DeleteAsset(Asset);
                System.Windows.MessageBox.Show("Zasób został pomyślnie usunięty", "Usuwanie Zasobu",MessageBoxButton.OK,MessageBoxImage.Information);

            }
            else
            {
                System.Windows.MessageBox.Show("Nie można usunąć zasobu ponieważ zostały wygenerowane dla niego raporty z analiz." +
                    "Usuń wszystkie raporty i ryzyka które związane z zasobem, aby móc go usunąć", "Usuwanie Zasobu",MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            DialogResult = true;
            CloseWindow();
            
        }
        
        private bool CanDeleteAsset()
        {
            return _assetRepository.CanDeleteAsset(Asset);
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
