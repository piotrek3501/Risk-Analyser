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
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Services;

namespace Risk_analyser.ViewModel
{
    public class DeleteAssetViewModel:ViewModelBase
    {
        private Asset _asset { get; set; }
        private AssetService AssetService { get; set; }
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

        public DeleteAssetViewModel(Asset asset ) { 
        
            _asset=asset;
            AssetService = MainWindowService.GetAssetService();
            DeleteAssetCommand = new RelayCommand(_=>DeleteAsset(),_=>Asset!=null);
            CancelAssetCommand = new RelayCommand(_=>CloseWindow(),_=>true);
        }

        private void DeleteAsset()
        {
            AssetService.DeleteAsset(Asset);
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
