using Microsoft.EntityFrameworkCore.Storage.Json;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using Risk_analyser.Services;
using Risk_analyser.View;
using Risk_analyser.View.UserControll.DetailsControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        //private  DataContext _context;
        public ICommand AddAssetCommand { get; }
        public ICommand AddRiskCommand { get; }
        public ICommand UpdateRiskCommand { get; }

        public ICommand AddControlRiskCommand { get; }
        public ICommand AddMitagationAction {  get; }
        public ICommand DeleteRiskCommand { get; }
        public ICommand DeleteAssetCommand { get; }
        public ICommand DeleteControlRiskCommand { get; }
        public ICommand DeleteMitagationActionCommand { get; }
        public ICommand UpdateAssetCommand { get; }
        public ICommand UpdateControlRiskCommand { get; }
        public ICommand UpdateMitagationActionCommand { get; }
        public ICommand CurrentCommandAddOrUpdateRisk { get; set; }
        public ICommand CurrentCommandAddOrUpdateControlRisk { get; set; }
        public ICommand CurrentCommandAddOrUpdateMitagation { get; set; }
        public ICommand AssetListDoubleClickCommand {  get; set; }
        public ICommand RiskListDoubleClickCommand { get;set; }
        private object _currentDetailsView { get; set; }
        private bool _isEditMode;

        private Asset _selectedAsset;
        private Risk _selectedRisk;
        private ControlRisk _selectedControlRisk;
        private MitagationAction _selectedMitagationAction;
        public object CurrentDetailsView
        {
            get => _currentDetailsView;
            private set
            {
                _currentDetailsView = value;
                OnPropertyChanged(nameof(CurrentDetailsView));
            }
        }
        public bool IsEditMode {
            get { return _isEditMode; }
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged();

                }
            }
        }

        // zgodnie z MVVM _assets to faktyczne dane z BD a Assets to tylko czesć lub calośc _assets udostepniona UI
        private ObservableCollection<Asset>_assets;
        public ObservableCollection<Asset> Assets { 
            get {return _assets;}
            set {
                if (_assets != value)
                {
                    _assets = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<Risk> _risks;
        public ObservableCollection<Risk> Risks
        {
            get { return _risks; }
            set
            {
                if (_risks != value)
                {
                    _risks = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<ControlRisk> _controls;
        public ObservableCollection<ControlRisk> Controls
        {
            get { return _controls; }
            set
            {
                if (_controls != value)
                {
                    _controls = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<MitagationAction> _mitagations;
        public ObservableCollection<MitagationAction> Mitagations
        {
            get { return _mitagations; }
            set
            {
                if( _mitagations != value)
                {
                    _mitagations = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Asset SelectedAsset
        {
            get { return _selectedAsset; }
            set
            {
                _selectedAsset = value;
                IsEditMode = _selectedAsset != null;
                _selectedRisk = null;
                Controls = null;
                CurrentCommandAddOrUpdateRisk = AddRiskCommand;
                OnPropertyChanged(nameof(CurrentCommandAddOrUpdateRisk));
                ChangeDetailsView();
                LoadRisks();
                OnPropertyChanged();

            }
        }
        public Risk SelectedRisk
        {
            get { return _selectedRisk; }
            set
            {
                _selectedRisk = value;
                IsEditMode = _selectedRisk != null;
                if (IsEditMode)
                {
                    CurrentCommandAddOrUpdateRisk = UpdateRiskCommand;
                }
                else
                {
                    CurrentCommandAddOrUpdateRisk = AddRiskCommand;
                }
                if (_selectedRisk != null)
                {
                    LoadControlsRisk();
                }
                else
                {
                    Controls = null;
                }
                ChangeDetailsView();
                OnPropertyChanged(nameof(CurrentCommandAddOrUpdateRisk));
                OnPropertyChanged();

            }
        }
        public ControlRisk SelectedControlRisk
        {
            get { return _selectedControlRisk; }
            set
            {
                _selectedControlRisk = value;
                IsEditMode = _selectedControlRisk != null;
                if (IsEditMode)
                {
                    CurrentCommandAddOrUpdateControlRisk = UpdateControlRiskCommand;
                }
                else
                {
                    CurrentCommandAddOrUpdateControlRisk= AddControlRiskCommand;
                }
                LoadMitagations();
                ChangeDetailsView();
                OnPropertyChanged(nameof(CurrentCommandAddOrUpdateControlRisk));
                OnPropertyChanged();
            }
        }
        public MitagationAction SelectedMitagation
        {
            get { return _selectedMitagationAction; }
            set
            {
                _selectedMitagationAction = value;
                IsEditMode= _selectedMitagationAction != null;
                if (IsEditMode)
                {
                    CurrentCommandAddOrUpdateMitagation = UpdateMitagationActionCommand;
                }
                else
                {
                    CurrentCommandAddOrUpdateMitagation = AddMitagationAction;
                }
                OnPropertyChanged(nameof(CurrentCommandAddOrUpdateMitagation));
                // LoadMitagations();
                ChangeDetailsView();
                OnPropertyChanged();
            }
        }



        public MainWindowViewModel()
        {
            MainWindowService.InitServices();
            //_context = context;
            AddAssetCommand = new RelayCommand(_ => ShowAddOrUpdateAssetWindow(false), _ => true);
            AddRiskCommand=new RelayCommand(_=>ShowAddOrUpdateRiskWindow(false), _ => SelectedAsset!=null);
            AddControlRiskCommand = new RelayCommand(_ => ShowAddOrUpdateControlRiskWindow(false), _ => SelectedRisk != null);
            AddMitagationAction=new RelayCommand(_=>ShowAddOrUpdateMitagationAction(false),_=>SelectedControlRisk!=null);
            UpdateMitagationActionCommand = new RelayCommand(_ => ShowAddOrUpdateMitagationAction(true),_=>IsEditMode == true);
            //zapewnie ze jesli nie wybrano zadnego ryzyka wtedy zamiast update zostanie on stworzony
            UpdateRiskCommand = new RelayCommand(_ => ShowAddOrUpdateRiskWindow(true), _ => IsEditMode==true);
            UpdateAssetCommand=new RelayCommand(_=>ShowAddOrUpdateAssetWindow(true),_=> IsEditMode==true);
            UpdateControlRiskCommand=new RelayCommand(_=>ShowAddOrUpdateControlRiskWindow(true),_=>IsEditMode==true);
            DeleteAssetCommand = new RelayCommand(_ => ShowDeletingAssetWindow(),_=>SelectedAsset!=null );
            DeleteRiskCommand = new RelayCommand(
                parameter=>ShowDeletingRiskWindow(parameter),
                parameter=>parameter!=null || SelectedRisk!=null
                );
            DeleteControlRiskCommand = new RelayCommand(
                        parameter => ShowDeletingControlRiskWindow(parameter),  // Przekaż element z CommandParameter
                        parameter => parameter != null || SelectedControlRisk != null  // Warunek: Musi istnieć kliknięty element lub wybrany element
             );
            //DeleteControlRiskCommand =new RelayCommand(_=> ShowDeletingControlRiskWindow(),_=>SelectedControlRisk!=null);
            DeleteMitagationActionCommand=new RelayCommand(parameter=>ShowDeletingMitgationActionWindow(parameter),_=>SelectedMitagation!=null);
            CurrentCommandAddOrUpdateControlRisk = AddControlRiskCommand;
            CurrentCommandAddOrUpdateMitagation=AddMitagationAction;
            AssetListDoubleClickCommand = new RelayCommand(_ => AssetElementDoubleCliked(), _ => true);
            RiskListDoubleClickCommand = new RelayCommand(_ => RiskElementDoubleCliked(), _ => true);


            LoadAsset();

        }

        private void RiskElementDoubleCliked()
        {
            SelectedControlRisk = null;
            SelectedMitagation = null;
            Mitagations = null;
            ChangeDetailsView();
        }

        private void AssetElementDoubleCliked()
        {
            SelectedRisk = null;
            SelectedControlRisk = null;
            SelectedMitagation = null;
            ChangeDetailsView();

        }

        private void ShowDeletingRiskWindow(object parameter)
        {
            var deleteRiskView = new DeleteRiskView();
            if (SelectedRisk == null)
                SelectedRisk = (Risk)parameter;
            var deleteRiskViewModel=new DeleteRiskViewModel(SelectedRisk);
            deleteRiskView.DataContext= deleteRiskViewModel;
            deleteRiskView.ShowDialog();
            if (deleteRiskViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Risk));
            }
            SelectedRisk = null;
            SelectedControlRisk = null;
            Controls = null;
            Mitagations = null;
            LoadRisks();
        }

        private void ShowDeletingAssetWindow()
        {
            var deleteAssetView = new DeleteAssetView();
            var deleteAssetViewModel=new DeleteAssetViewModel(SelectedAsset);
            deleteAssetView.DataContext= deleteAssetViewModel;
            deleteAssetView.ShowDialog();
            if (deleteAssetViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Asset));
            }
            SelectedAsset = null;
            SelectedRisk = null;
            SelectedControlRisk = null;
            Controls = null;
            Mitagations = null;
            Risks = null;
            LoadAsset();
        }
        private void ChangeDetailsView()
        {
            if(SelectedAsset!=null && SelectedRisk==null && SelectedControlRisk == null)
            {
                AssetDetails assetDetailsView=new AssetDetails();
                assetDetailsView.DataContext = new AssetDetailViewModel(SelectedAsset);
                CurrentDetailsView = assetDetailsView;
            }
            else if(SelectedAsset!=null && SelectedRisk!=null && SelectedControlRisk == null)
            {
                RiskDetails riskDetailsView=new RiskDetails();
                riskDetailsView.DataContext = new RiskDetailsViewModel(SelectedRisk);
                CurrentDetailsView=riskDetailsView;
            }
            else if (SelectedAsset != null && SelectedRisk != null && SelectedControlRisk != null && SelectedMitagation == null)
            {
                ControlRiskDetails controlRiskDetailsView=new ControlRiskDetails();
                controlRiskDetailsView.DataContext = new ControlRiskDetailsViewModel(SelectedControlRisk);
                CurrentDetailsView=controlRiskDetailsView;
            }
            else if(SelectedAsset != null && SelectedRisk != null && SelectedControlRisk != null && SelectedMitagation != null)
            {
                MitigationActionDetails mitigationActionDetails=new MitigationActionDetails();
                mitigationActionDetails.DataContext=new MitigationActionDetailsViewModel(SelectedMitagation);
                CurrentDetailsView = mitigationActionDetails;
            }
            else
            {
                CurrentDetailsView = null;
            }
        }
        private void ShowDeletingControlRiskWindow(object parameter)
        {
            var deleteControlRiskView = new DeleteControlRiskView();
            DeleteControlRiskViewModel deleteControlRiskViewModel;
            if(SelectedControlRisk==null)
            SelectedControlRisk = (ControlRisk)parameter;

            deleteControlRiskViewModel = new DeleteControlRiskViewModel(SelectedControlRisk, SelectedRisk);
            deleteControlRiskView.DataContext = deleteControlRiskViewModel;
            deleteControlRiskView.ShowDialog();
            if (deleteControlRiskViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(ControlRisk));
            }
            SelectedControlRisk = null;
            Mitagations = null;
            LoadControlsRisk();
        }
        private void ShowAddOrUpdateRiskWindow(bool Editing)
        {
            var updateOrAddRiskView = new UpdateOrAddRiskView();
            UpdateOrAddRiskViewModel updateOrAddRiskViewModel;
            if (Editing)
            {
                IsEditMode = true;
                updateOrAddRiskViewModel = new UpdateOrAddRiskViewModel(SelectedRisk);
                updateOrAddRiskView.DataContext = updateOrAddRiskViewModel;
            }
            else
            {
                IsEditMode = false;
                updateOrAddRiskViewModel = new UpdateOrAddRiskViewModel(SelectedAsset);
                updateOrAddRiskView.DataContext = updateOrAddRiskViewModel;
            }
            updateOrAddRiskView.ShowDialog();
            if (updateOrAddRiskViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Risk));
                
            }
            //IsEditMode = false;
            SelectedRisk = null;
            SelectedControlRisk = null;
            SelectedMitagation = null;
            Controls = null;
            Mitagations = null;
            CurrentCommandAddOrUpdateRisk = AddRiskCommand;
            OnPropertyChanged(nameof(CurrentCommandAddOrUpdateRisk));
            LoadRisks();
            OnPropertyChanged();
        }
        private void ShowAddOrUpdateControlRiskWindow(bool Editing)
        {
            var updateOrAddControlRiskView = new UpdateOrAddControlRiskView();
            UpdateOrAddControlRiskViewModel updateOrAddControlRiskViewModel;
            if (Editing)
            {
                  IsEditMode = true;
                 updateOrAddControlRiskViewModel = new UpdateOrAddControlRiskViewModel(SelectedControlRisk);
                updateOrAddControlRiskView.DataContext= updateOrAddControlRiskViewModel;
            }
            else
            {
                IsEditMode = false;
                 updateOrAddControlRiskViewModel = new UpdateOrAddControlRiskViewModel(SelectedAsset);
                updateOrAddControlRiskView.DataContext = updateOrAddControlRiskViewModel;
            }
            
            updateOrAddControlRiskView.ShowDialog();
            if (updateOrAddControlRiskViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(ControlRisk));
            }
            //IsEditMode = false;
            CurrentCommandAddOrUpdateControlRisk=AddControlRiskCommand;
            SelectedControlRisk = null;
            SelectedMitagation = null;
            OnPropertyChanged(nameof(CurrentCommandAddOrUpdateControlRisk));
            LoadControlsRisk();
            Mitagations = null;
            OnPropertyChanged();
        }
        private void ShowAddOrUpdateMitagationAction(bool Editing)
        {
            var updateOrAddMitagtionActionView = new UpdateOrAddMitigationActionView();
            UpdateOrAddMitagationViewModel updateOrAddMitagationViewModel;
            if (Editing)
            {
                IsEditMode = true;
                updateOrAddMitagationViewModel = new UpdateOrAddMitagationViewModel(SelectedMitagation);
                updateOrAddMitagtionActionView.DataContext = updateOrAddMitagationViewModel;
            }
            else
            {
                IsEditMode = false;
                updateOrAddMitagationViewModel = new UpdateOrAddMitagationViewModel(SelectedControlRisk);
                updateOrAddMitagtionActionView.DataContext = updateOrAddMitagationViewModel;
            }

            updateOrAddMitagtionActionView.ShowDialog();
            if (updateOrAddMitagationViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(MitagationAction));
            }
            //IsEditMode = false;
            CurrentCommandAddOrUpdateMitagation = AddMitagationAction;
            SelectedMitagation = null;
            Mitagations = null;
            OnPropertyChanged(nameof(CurrentCommandAddOrUpdateMitagation));
            LoadMitagations();
            OnPropertyChanged();
        }

        
        private void ShowDeletingMitgationActionWindow(object parameter)
        {
            var deleteMitigationActionView = new DeleteMitigationActionView();
            deleteMitigationActionView.DataContext = new DeleteMitigationActionViewModel((MitagationAction)parameter,SelectedControlRisk);
            deleteMitigationActionView.ShowDialog();
            if (deleteMitigationActionView.DialogResult == true)
            {
                OnPropertyChanged(nameof(MitagationAction));
            }
            SelectedMitagation = null;

            LoadMitagations();
            OnPropertyChanged(nameof(Mitagations));
            OnPropertyChanged();
        }
        private void LoadControlsRisk()
        {
            if (SelectedRisk != null)
            {
                Controls=MainWindowService.LoadControlRiskForRisk(SelectedRisk);
                
            }
        }
        private void LoadMitagations()
        {
            if(SelectedControlRisk != null)
            {
                Mitagations = MainWindowService.LoadMitigationForControlRisk(SelectedControlRisk);
            }
        }
        private void LoadAsset()
        {

            Assets = MainWindowService.LoadAllAssets();
           
        }
        private  void LoadRisks()
        {
            if (SelectedAsset != null)
            {
                Risks = new ObservableCollection<Risk>(MainWindowService.LoadAllRiskForAsset(SelectedAsset));
            }
        }
        private void ShowAddOrUpdateAssetWindow(bool Editing)
        {
            var updateOrAddAssetView = new UpdateOrAddAssetView();
            UpdateOrAddAssetViewModel updateOrAddAssetViewModel;
            if (Editing)
            {
                IsEditMode = true;
                updateOrAddAssetViewModel = new UpdateOrAddAssetViewModel(SelectedAsset);
                updateOrAddAssetView.DataContext = updateOrAddAssetViewModel;
            }
            else
            {
                IsEditMode = false;
                updateOrAddAssetViewModel = new UpdateOrAddAssetViewModel();
                updateOrAddAssetView.DataContext = updateOrAddAssetViewModel;
            }
            updateOrAddAssetView.ShowDialog();
            if (updateOrAddAssetViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Asset));
            }
            // IsEditMode = false;
            SelectedRisk = null;
            SelectedControlRisk = null;
            SelectedMitagation = null;
            Mitagations = null;
            Controls = null;
            Risks = null;
            SelectedAsset = null;
            LoadAsset();
        }
        
    }
}
