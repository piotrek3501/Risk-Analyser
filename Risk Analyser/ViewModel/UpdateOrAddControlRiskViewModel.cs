using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Repository;
using Risk_analyser.Model;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Risk_analyser.ViewModel
{
    public class UpdateOrAddControlRiskViewModel:ViewModelBase
    {
        private DataContext _context;
        private Risk _selectedRisk;
        private ControlRisk _selectedControlRisk;
        private ControlRiskRepository ControlRiskRepository { get; set; }
        private MitagationRepository MitagationRepository { get; set; }
        private RiskRepository RiskRepository { get; set; }
        private List<Risk>_ControlRisksList { get; set; }
        private List<MitagationAction> _ControlMitagationsList {  get; set; }
        public List<MitagationItem>MitagationItems { get; set; } =new List<MitagationItem>();
        public List<RiskItem>RiskItems { get; set; } = new List<RiskItem>();
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
        public List<Risk> ControlRisksList
        {
            get { return _ControlRisksList; }
            set
            {
                _ControlRisksList= value;
                OnPropertyChanged();
            }
        }
        public List<MitagationAction> ControlMitagationsList
        {
            get { 
               { return _ControlMitagationsList; } 
            }
            set
            {
                _ControlMitagationsList= value;
                OnPropertyChanged();
            }
        }
        public UpdateOrAddControlRiskViewModel(DataContext context)
        {
            _context = context;
            ControlRiskRepository = new ControlRiskRepository(_context);
            AddControlRiskCommand = new RelayCommand(_ => AddControlRisk(), _ => CanAddControlRisk());
            EditControlRiskCommand=new RelayCommand(_=>EditControlRisk(),_=>CanEditControlRisk());
            CancelControlRiskCommand = new RelayCommand(_ => CloseWindow(), _ => true);
            _ControlMitagationsList = ControlRiskRepository.GetAllMitagationActions();
            _ControlRisksList = ControlRiskRepository.GetAllRiskWithAsset();
            MitagationRepository = new MitagationRepository(_context);
            RiskRepository= new RiskRepository(_context);

            ControlRiskService=new ControlRiskService(_context);

        }

        public UpdateOrAddControlRiskViewModel(DataContext context,Asset selectedAsset):this(context)
        {
            //this._selectedRisk = RiskRepository.LoadRiskWithEntities(selectedRisk.RiskId);
           
            CurrentCommand = AddControlRiskCommand;
            foreach (Risk r in _ControlRisksList)
            {
                Risk risk=RiskRepository.LoadRiskWithEntities(r.RiskId);
                bool BelongToAssetOfSelectedAsset= selectedAsset.AssetId==risk.AssetId;
                RiskItems.Add(new RiskItem(risk.RiskId,risk.Name,risk.Description,risk.Asset.Name, BelongToAssetOfSelectedAsset));
            }
            foreach(MitagationAction action in _ControlMitagationsList)
            {
                MitagationAction mit = MitagationRepository.LoadMitagationWithEntities(action.MitagatioActionId);
                MitagationItems.Add(new MitagationItem(mit.MitagatioActionId,mit.Action,mit.Person,mit.DateOfAction,false));
            }
        }

        public UpdateOrAddControlRiskViewModel(DataContext context, ControlRisk selectedControlRisk) : this(context)
        {
            _selectedControlRisk = ControlRiskRepository.LoadControlRiskWithEntities(selectedControlRisk.ControlRiskId);
            CurrentCommand = EditControlRiskCommand;
            foreach (Risk r in _ControlRisksList)
            {
                Risk risk = RiskRepository.LoadRiskWithEntities(r.RiskId);
                RiskItems.Add(new RiskItem(risk.RiskId,risk.Name,risk.Description,risk.Asset.Name,_selectedControlRisk.Risks.Contains(risk)));
            }
            foreach (MitagationAction action in _ControlMitagationsList)
            {
                MitagationAction mit = MitagationRepository.LoadMitagationWithEntities(action.MitagatioActionId);
                MitagationItems.Add(new MitagationItem(mit.MitagatioActionId,mit.Action,
                    mit.Person,mit.DateOfAction,_selectedControlRisk.Mitagations.Contains(action)));
            }
        }

        private void AddControlRisk() {
            ControlRisk newControl = new ControlRisk()
            {
                Name =this.Name,
                Description=this.Description,
                Risks = ControlRiskService.GetSelectedRisks(RiskItems),
                Mitagations=ControlRiskService.GetSelectedMitigation(MitagationItems)
            };
            ControlRiskRepository.SaveRisk(newControl);
            System.Windows.MessageBox.Show("Środek został pomyślnie dodany", "Utworzenie Środka");
            DialogResult = true;
            CloseWindow();

        }
        private void EditControlRisk()
        {
            //SelectedControlRisk=ControlRiskRepository.LoadControlRiskWithEntities(SelectedControlRisk.ControlRiskId);
            ControlRisk updatedControl = new ControlRisk()
            {
                Name =Name,
                Description = Description,
            };
            updatedControl.Risks = ControlRiskService.GetSelectedRisks(RiskItems);
            updatedControl.Mitagations=ControlRiskService.GetSelectedMitigation(MitagationItems);
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
public class RiskItem
{
    public string Risk { get; set; }
    public long RiskId {  get; set; }
    public string RiskDesc { get; set; }
    public string Asset {  get; set; }
    public bool BelongToControlRisk { get; set; }
    public RiskItem(long id,string risk,string riskdesc,string asset,bool status)
    {
        RiskId = id;
        Risk = risk;
        Asset = asset;
        RiskDesc = riskdesc;
        BelongToControlRisk = status;
    }

}
public class MitagationItem
{
    public string Worker { get; set; }
    public string Action {  get; set; }
    public DateOnly DateOfAction { get; set; }
    public long IdMitagation { get; set; }
    public bool BelongToControlRisk { get; set; }
    public MitagationItem(long id,string action,string worker,DateOnly date,bool status)
    {
        IdMitagation = id;
        Worker = worker;
        Action = action;
        DateOfAction = date;
        BelongToControlRisk = status;
    }
}
