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
using System.Windows.Navigation;

namespace Risk_analyser.ViewModel
{
    public class UpdateOrAddAssetViewModel:ViewModelBase
    {
        private readonly DataContext _context;
        private AssetRepository assetRepository;
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
       // public bool IsEditMode {  get; private set; }


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
        public UpdateOrAddAssetViewModel(DataContext context)
        {
            _context = context;
            AddAssetCommand = new RelayCommand(_ => AddAsset(), _ => CanAddAsset());
            assetRepository = new AssetRepository(context);
            CancelAssetCommand = new RelayCommand(_=>CloseWindow(),_=>true);
            CurrentCommand = AddAssetCommand;
           // IsEditMode = false;

        }
        public UpdateOrAddAssetViewModel(DataContext context,Asset asset):this(context) 
        {
            _selectedAsset = asset;
            AssetName = _selectedAsset.Name;
            AssetDescription = _selectedAsset.Description;
            EditAssetCommand = new RelayCommand(_=>EditAsset(),_=>CanEditAsset());
            CurrentCommand = EditAssetCommand;
            // IsEditMode = true;

        }
        private bool CanAddAsset()
        {
            return !string.IsNullOrWhiteSpace(AssetName) && !string.IsNullOrWhiteSpace(AssetDescription);
        }
        private bool CanEditAsset()
        {
            return !string.IsNullOrWhiteSpace(AssetName) && !string.IsNullOrWhiteSpace(AssetDescription);
        }
        private void EditAsset()
        {
            Asset asset = SelectedAsset;
            asset.Name = AssetName;
            asset.Description = AssetDescription;
            asset.CreationTime = DateTime.Now;
           
           
            assetRepository.EditAsset(asset);
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
                FRAPAnalysis=new List<FRAPAnalysis>(),
                RhombusAnalysis=new List<RhombusAnalysis>(),
                CreationTime=DateTime.Now
            };

            assetRepository.AddAsset(newAsset);

            MessageBox.Show("Zasób został pomyślnie dodany","Zasób dodany");
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
