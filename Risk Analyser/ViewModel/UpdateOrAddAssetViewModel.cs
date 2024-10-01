using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.MVVM;
using Risk_analyser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Risk_analyser.ViewModel
{
    public class UpdateOrAddAssetViewModel:ViewModelBase
    {
        private AssetService _assetService;
        private string _name;
        private string _description;
        private Asset _selectedAsset;
        public Asset SelectedAsset
        {
            get { return _selectedAsset; }
            set { 
                _selectedAsset = value;
                OnPropertyChanged();
            }
        }
        public bool DialogResult {  get; private set; }

        public string AssetName
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string AssetDescription
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddAssetCommand { get; }
        public ICommand CancelAssetCommand { get; }
        public ICommand EditAssetCommand { get; }
        public ICommand CurrentCommand { get; set; }
        public UpdateOrAddAssetViewModel()
        {
            _assetService=MainWindowService.GetAssetService();
            AddAssetCommand = new RelayCommand(_ => AddAsset(), _ =>true);
            CancelAssetCommand = new RelayCommand(_=>CloseWindow(),_=>true);
            CurrentCommand = AddAssetCommand;
           // IsEditMode = false;

        }
        public UpdateOrAddAssetViewModel(Asset asset ) :this() 
        {
            _selectedAsset = asset;
            AssetName = _selectedAsset.Name;
            AssetDescription = _selectedAsset.Description;
            EditAssetCommand = new RelayCommand(_=>EditAsset(),_=>true);
            CurrentCommand = EditAssetCommand;
            // IsEditMode = true;

        }
       
        private void EditAsset()
        {
            //Asset asset = SelectedAsset;
            Asset ChangedAsset = new Asset();
            ChangedAsset.AssetId = SelectedAsset.AssetId;
            ChangedAsset.Name = AssetName;
            ChangedAsset.Description = AssetDescription;
            //ChangedAsset.CreationTime = DateTime.Now;
           
            _assetService.EditAsset(ChangedAsset);
            DialogResult = true;
            CloseWindow();
        }
        private void AddAsset()
        {
            // Dodawanie nowego aktywa do bazy danych
            var newAsset = new Asset
            {
                Name = AssetName,
                Description = AssetDescription,
                Risks = new List<Risk>(),
                FRAPAnalysis=new FRAPAnalysis(),
                RhombusAnalysis=new RhombusAnalysis()
                {
                    Results=new List<RhombusDocument>()
                },
                CreationTime=DateTime.Now,
            };

            _assetService.AddAsset(newAsset);

            DialogResult = true;
            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
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
