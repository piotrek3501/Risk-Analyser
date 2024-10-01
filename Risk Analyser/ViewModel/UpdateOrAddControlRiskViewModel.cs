using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using Risk_analyser.Services;
using System.Windows;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class UpdateOrAddControlRiskViewModel:ViewModelBase
    {
        private DataContext _context;
        private Risk _selectedRisk;
        private ControlRisk _selectedControlRisk;
        private List<Risk>_RisksList { get; set; }
        private List<MitagationAction> _MitagationsList {  get; set; }
        public List<MitagationDataGridItem>MitagationItems { get; set; } =new List<MitagationDataGridItem>();
        public List<RiskDataGridItem>RiskItems { get; set; } = new List<RiskDataGridItem>();
        public bool DialogResult { get; private set; }
        public ICommand AddControlRiskCommand { get; set; }
        public ICommand EditControlRiskCommand { get; set; }
        public ICommand CurrentCommand { get; set; }
        public ICommand CancelControlRiskCommand { get; set; }
        private ControlRiskService ControlRiskService { get; set; }
        private string _name {  get; set; }
        private string _description { get; set; }
        public string Name {  
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get { return _description; }
            set {
                _description= value; 
                OnPropertyChanged(); 
            }
        }


        public ControlRisk SelectedControlRisk {
            get { return  _selectedControlRisk; }
            set
            {
                _selectedControlRisk = value;
                OnPropertyChanged();
            }
        }
        public Risk SelectedRisk
        {
            get { return _selectedRisk; }
            set { 
                _selectedRisk= value;
                OnPropertyChanged();
            }
        }
        public List<Risk> RisksList
        {
            get { return _RisksList; }
            set
            {
                _RisksList= value;
                OnPropertyChanged();
            }
        }
        public List<MitagationAction> MitagationsList
        {
           get { 
               { return _MitagationsList; } 
            }
           set
            {
               _MitagationsList= value;
               OnPropertyChanged();
           }
        }
        public UpdateOrAddControlRiskViewModel()
        {
            
            AddControlRiskCommand = new RelayCommand(_ => AddControlRisk(), _ => CanAddControlRisk());
            EditControlRiskCommand=new RelayCommand(_=>EditControlRisk(),_=>CanEditControlRisk());
            CancelControlRiskCommand = new RelayCommand(_ => CloseWindow(), _ => true);
            ControlRiskService =MainWindowService.GetControlRiskService();
            _MitagationsList = ControlRiskService.GetAllMitagationActions();
            _RisksList = ControlRiskService.GetAllRiskWithAsset();




        }

        public UpdateOrAddControlRiskViewModel(Asset selectedAsset) :this()
        {
            //this._selectedRisk = RiskRepository.LoadRiskWithEntities(selectedRisk.RiskId);
           
            CurrentCommand = AddControlRiskCommand;
            RiskItems=ControlRiskService.InitRisksForDataGrid(RisksList, selectedAsset);
            MitagationItems = ControlRiskService.InitMitigationForDataGrid(MitagationsList);
        }

        public UpdateOrAddControlRiskViewModel(ControlRisk selectedControlRisk) : this()
        {
            _selectedControlRisk = ControlRiskService.LoadControlRiskWithEntities(selectedControlRisk.ControlRiskId);
            Name = _selectedControlRisk.Name;
            Description = _selectedControlRisk.Description;
            CurrentCommand = EditControlRiskCommand;
            RiskItems=ControlRiskService.InitRisksForDataGrid(RisksList,_selectedControlRisk);
            //MitagationItems=ControlRiskService.InitMitigationForDataGrid(MitagationsList,_selectedControlRisk);
            MitagationItems = ControlRiskService.InitMitigationForDataGrid(MitagationsList, _selectedControlRisk);

        }

        private void AddControlRisk() {
            ControlRisk newControl = new ControlRisk()
            {
                Name = this.Name,
                Description = this.Description,
                Risks = ControlRiskService.GetSelectedRisks(RiskItems),
                Mitagations = ControlRiskService.GetSelectedMitigation(MitagationItems),
                CreationDate = DateTime.Now
            };
            ControlRiskService.AddControlRisk(newControl);
            DialogResult = true;
            CloseWindow();

        }
        private void EditControlRisk()
        {
            //SelectedControlRisk=ControlRiskRepository.LoadControlRiskWithEntities(SelectedControlRisk.ControlRiskId);
            ControlRisk updatedControl = new ControlRisk()
            {
                ControlRiskId=SelectedControlRisk.ControlRiskId,
                Name =this.Name,
                Description = this.Description,
            };
            updatedControl.Risks = ControlRiskService.GetSelectedRisks(RiskItems);
            updatedControl.Mitagations=ControlRiskService.GetSelectedMitigation(MitagationItems);
            ControlRiskService.EditControlRisk(updatedControl);
            DialogResult = true;
            CloseWindow();
        }
        private bool CanEditControlRisk()
        {
            return _selectedControlRisk != null;
        }

        private bool CanAddControlRisk()
        {
            return _selectedRisk != null;

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

