using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Repository;
using Risk_analyser.Model;
using Risk_analyser.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static Risk_analyser.Model.Rhombus;

namespace Risk_analyser.ViewModel
{
    public class UpdateOrAddRiskViewModel:ViewModelBase
    {
        private DataContext _context { get; set; }
        private RiskRepository RiskRepository { get; set; }
        private Asset _selectedAsset { get; set; }
        private Risk _selectedRisk { get; set; }
        private string _name;
        private string _description;
        private Propability _selectedProbability;
        private Priority _selectedPriority;
        private Vulnerability _selectedVulnerability;
        private PotencialEffect _selectedPotencialEffect;
        private bool _selectedTypeRiskINT;
        private bool _selectedTypeRiskCON;
        private bool _selectedTypeRiskAVA;
        private TypeInovation _selectedTypeInovation { get; set; }
        private TypeTechnology _selectedTypeTechnology { get; set; }
        private TypeComplexity _selectedTypeComplexity { get; set; }
        private TypeRate _selectedTypeRate {  get; set; }
        private ObservableCollection<TypeInovation> _typeInovationsItems;
        private ObservableCollection<TypeTechnology> _typeTechnologyItems;
        private ObservableCollection<TypeComplexity> _typeComplexityItems;
        private ObservableCollection<TypeRate> _typeRateItems;
        private ObservableCollection<Propability> _propabilityItems;
        private ObservableCollection<Priority> _priorityItems;
        private ObservableCollection<PotencialEffect> _potencialEffectItems;
        private ObservableCollection<Vulnerability> _vulnerabilityItems;

        public bool DialogResult { get; private set; }
        public ICommand AddRiskCommand { get; set; }
        public ICommand EditRiskCommand { get; set; }
        public ICommand CurrentCommand { get; set; }
        public ICommand CancelRiskCommand { get; set; }
        public Asset SelectedAsset
        {
            get { return _selectedAsset; }
            set
            {
                _selectedAsset = value;
                //AddRiskCommand = new RelayCommand(_ => AddRisk(), _ => CanAddRisk());
                //CurrentCommand = AddRiskCommand;
                //OnPropertyChanged(nameof(CurrentCommand));
                OnPropertyChanged();
            }
        }

        public string Name { get { return _name; } 
            set {
                _name = value;
                OnPropertyChanged();
            } 
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        public Propability SelectedProbability
        {
            get { return _selectedProbability; }
            set
            {
                _selectedProbability = value;
                OnPropertyChanged();
            }
        }
        public Priority SelectedPriority
        {
            get { return _selectedPriority; }
            set
            {
                if (SelectedVurnelability!=null && SelectedPotencialEffect != null)
                {
                    if ((SelectedPotencialEffect == PotencialEffect.High) && (SelectedVurnelability == Vulnerability.High))
                    {
                        _selectedPriority = Priority.A;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.Medium) && (SelectedVurnelability == Vulnerability.High))
                    {
                        _selectedPriority = Priority.B;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.Low) && (SelectedVurnelability == Vulnerability.High))
                    {
                        _selectedPriority = Priority.C;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.High) && (SelectedVurnelability == Vulnerability.Medium))
                    {
                        _selectedPriority = Priority.B;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.Medium) && (SelectedVurnelability == Vulnerability.Medium))
                    {
                        _selectedPriority = Priority.B;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.Low) && (SelectedVurnelability == Vulnerability.Medium))
                    {
                        _selectedPriority = Priority.C;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.High) && (SelectedVurnelability == Vulnerability.Low))
                    {
                        _selectedPriority = Priority.C;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.Medium) && (SelectedVurnelability == Vulnerability.Low))
                    {
                        _selectedPriority = Priority.C;
                    }
                    else if ((SelectedPotencialEffect == PotencialEffect.Low) && (SelectedVurnelability == Vulnerability.Low))
                    {
                        _selectedPriority = Priority.D;
                    }
                    OnPropertyChanged();
                }
            }
        }
        public Vulnerability SelectedVurnelability
        {
            get { return _selectedVulnerability; }
            set
            {
                _selectedVulnerability = value;
                SelectedPriority = SelectedPriority;
                OnPropertyChanged();
            }
        }

        public PotencialEffect SelectedPotencialEffect
        {
            get { return _selectedPotencialEffect; }
            set
            {
                _selectedPotencialEffect = value;
                SelectedPriority = SelectedPriority;
                OnPropertyChanged();
            }
        }
      
        public bool SelectedTypeRiskINT
        {
            get { return _selectedTypeRiskINT; }
            set
            {
                _selectedTypeRiskINT = value;
                OnPropertyChanged();
            }
        }
        private bool SelectedTypeRiskCON
        {
            get { return _selectedTypeRiskCON; }
            set
            {
                _selectedTypeRiskCON = value;
                OnPropertyChanged();
            }
        }
        private bool SelectedTypeRiskAVA
        {
            get { return _selectedTypeRiskAVA; }
            set
            {
                _selectedTypeRiskAVA = value;
                OnPropertyChanged();
            }
        }
        public TypeInovation SelectedTypeInovation
        {
            get { return _selectedTypeInovation; }
            set
            {
                _selectedTypeInovation = value;
                OnPropertyChanged();
            }
        }
        public TypeTechnology SelectedTypeTechnology
        {
            get { return _selectedTypeTechnology; }
            set
            {
                _selectedTypeTechnology = value;
                OnPropertyChanged();
            }
        }
        public TypeComplexity SelectedTypeComplexity
        {
            get { return _selectedTypeComplexity; }
            set
            {
                _selectedTypeComplexity = value;
                OnPropertyChanged();
            }
        }
        public TypeRate SelectedTypeRate
        {
            get { return _selectedTypeRate; }
            set
            {
                _selectedTypeRate = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TypeInovation> TypeInovationItems
        {
            get { return _typeInovationsItems; }
            set
            {
                _typeInovationsItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TypeTechnology> TypeTechnologyItems
        {
            get { return _typeTechnologyItems; }
            set
            {
                _typeTechnologyItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TypeComplexity> TypeComplexityItems
        {
            get { return _typeComplexityItems; }
            set
            {
                _typeComplexityItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TypeRate> TypeRateItems
        {
            get { return _typeRateItems; }
            set
            {
                _typeRateItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Propability> PropabilityItems
        {
            get { return _propabilityItems; }
            set
            {
                _propabilityItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Priority> PriorityItems
        {
            get { return _priorityItems; }
            set
            {
                _priorityItems = value;
                OnPropertyChanged();
            }
        }
       public ObservableCollection<PotencialEffect> PotencialEffectItems
        {
            get { return _potencialEffectItems; }
            set
            {
                _potencialEffectItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Vulnerability> VulnerabilityItems
        {
            get { return _vulnerabilityItems; }
            set
            {
                _vulnerabilityItems = value;
                OnPropertyChanged();
            }
        }
        public Risk SelectedRisk
        {
            get { return _selectedRisk; }
            set
            {
                _selectedRisk = value;
                //EditRiskCommand = new RelayCommand(_ => EditRisk(), _ => CanEditRisk());
                //CurrentCommand = EditRiskCommand;
                OnPropertyChanged();
            }
        }
        public UpdateOrAddRiskViewModel(DataContext context,Asset selectedAsset):this(context)
        {
            SelectedAsset = selectedAsset;
            CurrentCommand = AddRiskCommand;
        }

        public UpdateOrAddRiskViewModel(DataContext context,Risk selectedRisk):this(context)
        {
            SelectedRisk = RiskRepository.LoadRiskWithEntities(selectedRisk.RiskId);
            SelectedAsset = SelectedRisk.Asset;
            Name = SelectedRisk.Name;
            Description = SelectedRisk.Description;
            SelectedProbability = SelectedRisk.Propability;
            SelectedPriority = SelectedRisk.Priority;
            SelectedPotencialEffect= SelectedRisk.PotencialEffect;
            SelectedVurnelability = SelectedRisk.Vulnerability;
            
            if (SelectedRisk.Types.Any(x=>x.Type==TypeRiskValues.INT))
            {
                SelectedTypeRiskINT = true;
            }
            else
                SelectedTypeRiskINT = false;
            if (SelectedRisk.Types.Any(x => x.Type == TypeRiskValues.CON))
            {
                SelectedTypeRiskCON = true;
            }
            else
                SelectedTypeRiskCON = false;
            if (SelectedRisk.Types.Any(x => x.Type == TypeRiskValues.AVA))
            {
                SelectedTypeRiskAVA = true;
            }
            else
                SelectedTypeRiskAVA = false;


            SelectedTypeInovation = SelectedRisk.RhombusParams.Inovation; 
            SelectedTypeTechnology= SelectedRisk.RhombusParams.Technology;
            SelectedTypeComplexity = SelectedRisk.RhombusParams.Complexity;
            SelectedTypeRate = SelectedRisk.RhombusParams.Rate;

            CurrentCommand = EditRiskCommand;

        }
        public UpdateOrAddRiskViewModel(DataContext context)
        {

            TypeInovationItems = new ObservableCollection<TypeInovation>(Enum.GetValues(typeof(TypeInovation)).Cast<TypeInovation>());
            TypeTechnologyItems=new ObservableCollection<TypeTechnology>(Enum.GetValues(typeof(TypeTechnology)).Cast<TypeTechnology>());
            TypeComplexityItems=new ObservableCollection<TypeComplexity>(Enum.GetValues(typeof(TypeComplexity)).Cast<TypeComplexity>());
            TypeRateItems=new ObservableCollection<TypeRate>(Enum.GetValues(typeof(TypeRate)).Cast<TypeRate>());
            PropabilityItems=new ObservableCollection<Propability>(Enum.GetValues(typeof(Propability)).Cast<Propability>());
            PriorityItems=new ObservableCollection<Priority>(Enum.GetValues(typeof(Priority)).Cast<Priority>());
            PotencialEffectItems=new ObservableCollection<PotencialEffect>(Enum.GetValues(typeof(PotencialEffect)).Cast<PotencialEffect>());
            VulnerabilityItems=new ObservableCollection<Vulnerability>(Enum.GetValues(typeof(Vulnerability)).Cast<Vulnerability>());
            
            RiskRepository = new RiskRepository(context);
            CancelRiskCommand = new RelayCommand(_ => CloseWindow(), _ => true);
            EditRiskCommand = new RelayCommand(_ => EditRisk(), _ => CanEditRisk());
            AddRiskCommand = new RelayCommand(_ => AddRisk(), _ => CanAddRisk());



        }
        private bool CanEditRisk()
        {
            return _selectedRisk != null;
        }

        private bool CanAddRisk()
        {
            return  _selectedAsset != null;

        }
        
        public void EditRisk()
        {
            var updatedRisk = SelectedRisk;
            updatedRisk.Name = Name;
            updatedRisk.Description = Description;
            updatedRisk.RhombusParams.Inovation = SelectedTypeInovation;
            updatedRisk.RhombusParams.Complexity=SelectedTypeComplexity;
            updatedRisk.RhombusParams.Technology=SelectedTypeTechnology;
            updatedRisk.RhombusParams.Rate=SelectedTypeRate;
            updatedRisk.Vulnerability = SelectedVurnelability;
            updatedRisk.Propability = SelectedProbability;
            updatedRisk.PotencialEffect = SelectedPotencialEffect;
            updatedRisk.Types.Clear();
            if (SelectedTypeRiskINT == true)
            {
                updatedRisk.Types.Add(new RiskType { Type = TypeRiskValues.INT, RiskId = updatedRisk.RiskId });
            }
            if (SelectedTypeRiskCON == true)
            {
                updatedRisk.Types.Add(new RiskType { Type = TypeRiskValues.CON, RiskId = updatedRisk.RiskId });

            }
            if (SelectedTypeRiskAVA == true)
            {
                updatedRisk.Types.Add(new RiskType { Type = TypeRiskValues.AVA, RiskId = updatedRisk.RiskId });
            }
            // wywolanie settar aby ten okreslij wartosc dla priority
            updatedRisk.Priority = SelectedPriority;   
            RiskRepository.EditRisk(updatedRisk);
            System.Windows.MessageBox.Show("Ryzyko zostało pomyślnie zmienione", "Edycja Ryzyka");
            DialogResult = true;
            CloseWindow();
        }

        private void AddRisk()
        {

            var newRisk = new Risk()
            {
                Name = Name,
                Description = Description,
                Asset = SelectedAsset,
                AssetId = SelectedAsset.AssetId,
                Vulnerability = SelectedVurnelability,
                Types=new List<RiskType>(),
                Controls = new List<ControlRisk>(),
                Propability = SelectedProbability,
                PotencialEffect = SelectedPotencialEffect,
                CreationTime=DateTime.Now,
                RhombusParams = new Rhombus()
                {
                    Complexity = SelectedTypeComplexity,
                    Inovation = SelectedTypeInovation,
                    Technology = SelectedTypeTechnology,
                    Rate = SelectedTypeRate,

                },
                DoneMitigationActions = new List<ActionReport>()

            };
            if (SelectedTypeRiskINT == true)
            {
                newRisk.Types.Add(new RiskType { Type = TypeRiskValues.INT, RiskId = newRisk.RiskId });
            }
            if (SelectedTypeRiskCON == true)
            {
                newRisk.Types.Add(new RiskType { Type = TypeRiskValues.CON, RiskId = newRisk.RiskId });

            }
            if (SelectedTypeRiskAVA == true)
            {
                newRisk.Types.Add(new RiskType{ Type = TypeRiskValues.AVA,RiskId=newRisk.RiskId });
            }
            
            newRisk.RhombusParams.Risk = newRisk;
            newRisk.RhombusParams.RiskId = newRisk.RiskId;
            newRisk.Priority = SelectedPriority;
            RiskRepository.SaveRisk(newRisk);

            System.Windows.MessageBox.Show("Ryzyko zostało pomyślnie dodane", "Utworzenie Ryzyka");
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
