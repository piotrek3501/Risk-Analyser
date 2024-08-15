using Microsoft.EntityFrameworkCore.Storage.Json;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Model;
using Risk_analyser.MVVM;
using Risk_analyser.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private  DataContext _context;
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
        private bool _isEditMode;

        private Asset _selectedAsset;
        private Risk _selectedRisk;
        private ControlRisk _selectedControlRisk;
        private MitagationAction _selectedMitagationAction;
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
                CurrentCommandAddOrUpdateRisk = AddRiskCommand;
                OnPropertyChanged(nameof(CurrentCommandAddOrUpdateRisk));
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
                OnPropertyChanged(nameof(CurrentCommandAddOrUpdateRisk));
                LoadControlsRisk();
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
                    CurrentCommandAddOrUpdateControlRisk = UpdateControlRiskCommand;
                }
                else
                {
                    CurrentCommandAddOrUpdateControlRisk = AddControlRiskCommand;
                }
                OnPropertyChanged(nameof(CurrentCommandAddOrUpdateControlRisk));
                LoadMitagations();
                OnPropertyChanged();
            }
        }



        public MainWindowViewModel(DataContext context)
        {
            _context = context;
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
            DeleteRiskCommand=new RelayCommand(ShowDeletingRiskWindow,_=>SelectedRisk!=null);
            DeleteControlRiskCommand=new RelayCommand(_=>ShowDeletingControlRiskWindow(),_=>SelectedControlRisk!=null);
            DeleteMitagationActionCommand=new RelayCommand(_=>ShowDeletingMitgationActionWindow(),_=>SelectedMitagation!=null);
            CurrentCommandAddOrUpdateControlRisk = AddControlRiskCommand;

            LoadData();

        }

        private void ShowDeletingRiskWindow(object parameter)
        {
            var deleteRiskView = new DeleteRiskView();
            var deleteRiskViewModel=new DeleteRiskViewModel(_context,parameter as Risk);
            deleteRiskView.DataContext= deleteRiskViewModel;
            deleteRiskView.ShowDialog();
            if (deleteRiskViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Risk));
            }
            SelectedRisk = null;
            LoadRisks();
        }

        private void ShowDeletingAssetWindow()
        {
            var deleteAssetView = new DeleteAssetView();
            var deleteAssetViewModel=new DeleteAssetViewModel(_context,SelectedAsset);
            deleteAssetView.DataContext= deleteAssetViewModel;
            deleteAssetView.ShowDialog();
            if (deleteAssetViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Asset));
            }
            SelectedAsset = null;
            LoadData();
        }
        private void ShowAddOrUpdateRiskWindow(bool Editing)
        {
            var updateOrAddRiskView = new UpdateOrAddRiskView();
            UpdateOrAddRiskViewModel updateOrAddRiskViewModel;
            if (Editing)
            {
                IsEditMode = true;
                updateOrAddRiskViewModel = new UpdateOrAddRiskViewModel(_context, SelectedRisk);
                updateOrAddRiskView.DataContext = updateOrAddRiskViewModel;
            }
            else
            {
                IsEditMode = false;
                updateOrAddRiskViewModel = new UpdateOrAddRiskViewModel(_context, SelectedAsset);
                updateOrAddRiskView.DataContext = updateOrAddRiskViewModel;
            }
            updateOrAddRiskView.ShowDialog();
            if (updateOrAddRiskViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Risk));
            }
            //IsEditMode = false;
            SelectedRisk = null;
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
                 updateOrAddControlRiskViewModel = new UpdateOrAddControlRiskViewModel(_context,SelectedControlRisk);
                updateOrAddControlRiskView.DataContext= updateOrAddControlRiskViewModel;
            }
            else
            {
                IsEditMode = false;
                 updateOrAddControlRiskViewModel = new UpdateOrAddControlRiskViewModel(_context,SelectedAsset);
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
            OnPropertyChanged(nameof(CurrentCommandAddOrUpdateControlRisk));
            LoadControlsRisk();
            OnPropertyChanged();
        }
        private void ShowAddOrUpdateMitagationAction(bool Editing)
        {
            var updateOrAddMitagtionActionView = new UpdateOrAddMitgationActionView();
            UpdateOrAddMitagationViewModel updateOrAddMitagationViewModel;
            if (Editing)
            {
                IsEditMode = true;
                updateOrAddMitagationViewModel = new UpdateOrAddMitagationViewModel(_context,SelectedMitagation);
                updateOrAddMitagtionActionView.DataContext = updateOrAddMitagationViewModel;
            }
            else
            {
                IsEditMode = false;
                updateOrAddMitagationViewModel = new UpdateOrAddMitagationViewModel(_context,SelectedControlRisk);
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
            OnPropertyChanged(nameof(CurrentCommandAddOrUpdateMitagation));
            LoadMitagations();
            OnPropertyChanged();
        }

        private void ShowDeletingControlRiskWindow()
        {
            throw new NotImplementedException();
        }
        private void ShowDeletingMitgationActionWindow()
        {
            throw new NotImplementedException();
        }
        private void LoadControlsRisk()
        {
            if (SelectedRisk != null)
            {
                Controls = new ObservableCollection<ControlRisk>(_context.ControlsRisks.Where(x => x.Risks.Contains(SelectedRisk)));
            }
        }
        private void LoadMitagations()
        {
            if(SelectedControlRisk != null)
            {
                Mitagations = new ObservableCollection<MitagationAction>(_context.MitagationActions
                   .Where(x=>x.ControlRisks.Contains(SelectedControlRisk)));
            }
        }
        private void LoadData()
        {
            Assets = new ObservableCollection<Asset>(_context.Assets.ToList());
           
        }
        private  void LoadRisks()
        {
            if (SelectedAsset != null)
            {
                Risks = new ObservableCollection<Risk>(_context.Risks.Where(x => x.Asset.AssetId == SelectedAsset.AssetId).ToList());
            }
        }
        private void ShowAddOrUpdateAssetWindow(bool Editing)
        {
            var updateOrAddAssetView = new UpdateOrAddAssetView();
            UpdateOrAddAssetViewModel updateOrAddAssetViewModel;
            if (Editing)
            {
                IsEditMode = true;
                updateOrAddAssetViewModel = new UpdateOrAddAssetViewModel(_context,SelectedAsset);
                updateOrAddAssetView.DataContext = updateOrAddAssetViewModel;
            }
            else
            {
                IsEditMode = false;
                updateOrAddAssetViewModel = new UpdateOrAddAssetViewModel(_context);
                updateOrAddAssetView.DataContext = updateOrAddAssetViewModel;
            }
            updateOrAddAssetView.ShowDialog();
            if (updateOrAddAssetViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(Asset));
            }
           // IsEditMode = false;
            SelectedAsset = null;
            LoadData();
        }
        
    }
}
