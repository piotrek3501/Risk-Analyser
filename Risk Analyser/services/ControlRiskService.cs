using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Risk_analyser.Data;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.Services;
using System.Windows;

namespace Risk_analyser.services
{
    public  class ControlRiskService
    {
        private  RiskService RiskService;
        private   MitigationActionService MitigationActionService;
        private  ControlRiskRepository ControlRiskRepository;
        private FRAPAnalysisService FRAPAnalysisService;
        private RhombusAnalysisService RhombusAnalysisService;
        private Risk SelectedRisk {  get; set; }
        private string ErrorMessage;

        public ControlRiskService(ControlRiskRepository controlRiskRepository)
        {
            RiskService = MainWindowService.GetRiskService();
            MitigationActionService = MainWindowService.GetMitigationActionService();
            ControlRiskRepository = controlRiskRepository;
            //FRAPAnalysisService = MainWindowService.GetFRAPAnalysisService();
            //RhombusAnalysisService = MainWindowService.GetRhombusAnalysisService();
        }
        public void SetFRAPAnalysisService()
        {
            FRAPAnalysisService = MainWindowService.GetFRAPAnalysisService();
        }
        public void SetRhombusAnalysisService()
        {
            RhombusAnalysisService = MainWindowService.GetRhombusAnalysisService();
        }
        public void SetMitigationActionService()
        {
            MitigationActionService = MainWindowService.GetMitigationActionService();
        }
        public List<Risk> GetSelectedRisks(List<RiskDataGridItem>selectedRisks)
        {
            return RiskService.GetRisksFromSelection(selectedRisks);
        }
        public List<MitagationAction> GetSelectedMitigation(List<MitagationDataGridItem> selectedMitigation)
        {
            return MitigationActionService.GetMitgationFromSelection(selectedMitigation);
        }
        public List<ControlRisk> GetAllControlRiskForRisk(Risk risk)
        {
            return ControlRiskRepository.GetAllControlRisksForRisk(risk);
        }
        public List<ControlRisk> GetAllUniqueControlRiskForRisks(List<Risk> risks)
        {
            List<ControlRisk>UniqueControls= new List<ControlRisk>();
            foreach(Risk r in risks)
            {
                List<ControlRisk> Controls = ControlRiskRepository.GetAllControlRisksForRisk(r);
                foreach(ControlRisk c in Controls)
                {
                    if (!UniqueControls.Contains(c))
                    {
                        UniqueControls.Add(c);
                    }
                }
            }
            return UniqueControls;
        }
        public List<MitagationAction> GetAllUniqueMitagationForControls(List<ControlRisk> controlRisks)
        {
            List<MitagationAction> UniqueMitigation = new List<MitagationAction>();
            MitigationActionService mitigationActionService=MainWindowService.GetMitigationActionService();
            foreach (ControlRisk c in controlRisks)
            {
                List<MitagationAction>mitagationActionsList = mitigationActionService.GetMitagationsFromControlRisk(c);
                foreach (MitagationAction m in mitagationActionsList)
                {
                    if (!UniqueMitigation.Contains(m))
                    {
                        UniqueMitigation.Add(m);
                    }
                }
            }
            return UniqueMitigation;
        }

        public List<Risk> GetRisksForControlRisk(long controlId)
        {
            return RiskService.GetRiskForControlRisk(controlId);
        }
        public List<MitagationAction> GetAllMitagationActions()
        {
            return MitigationActionService.GetAllMitigation();
        }
      
        public List<Risk> GetAllRiskWithAsset()
        {
            return RiskService.GetAllRiskWithAsset();
        }
        private Risk LoadRiskWithEntities(long id)
        {
            return RiskService.LoadRiskWithEntities(id);
        }
        public void AddControlRisk(ControlRisk control)
        {
            if (CanAddControlRisk(control))
            {
                ControlRiskRepository.AddControlRisk(control);
                System.Windows.MessageBox.Show("Środek został pomyślnie dodany", "Utworzenie Środka", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Środek  nie został pomyślnie dodany ponieważ:\n"+ErrorMessage, "Tworzenie Środka", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                ErrorMessage = null;
            }

        }
      
        private bool CanAddControlRisk(ControlRisk control)
        {
            ErrorMessage = "";
            bool IsEmptyName = string.IsNullOrWhiteSpace(control.Name);
            if (IsEmptyName)
            {
                ErrorMessage = ErrorMessage + "->Nazwa środka nie może być pusta\n";
            }
            bool IsEmptyDescription= string.IsNullOrWhiteSpace(control.Description);
            if (IsEmptyDescription)
            {
                ErrorMessage = ErrorMessage + "->Opis środka nie może być pusty\n";
            }
            var IsEmptyRiskList=!control.Risks.Any();
            if (IsEmptyRiskList)
            {
                ErrorMessage = ErrorMessage + "->Dla środka nie przypisanano żadnego ryzyka\n";
            }
            return !(IsEmptyRiskList || IsEmptyName || IsEmptyDescription);
        }
        private bool CanDeleteOrEditControlRisk(ControlRisk control,bool editing)
        {
            ErrorMessage = "";
            bool AnyFrapAnalysis = false;
            bool AnyRhombusAnalysis = false;
            Risk risk = RiskService.LoadRiskWithEntities(control.Risks.First().RiskId);
            Asset asset = risk.Asset;
            AnyFrapAnalysis = FRAPAnalysisService.OlderThanAllFRAPsInAsset(asset,control);
            AnyRhombusAnalysis = RhombusAnalysisService.OlderThanAllRhombusInAsset(asset,control);

            if (editing)
            {   
                bool IsFulfillAddingConditions= CanAddControlRisk(control);
               
                if (AnyFrapAnalysis)
                {
                    ErrorMessage = ErrorMessage + "->Ten środek został uwzględniony w jednej z analiz FRAP .Usuń tą analizę aby " +
                                      "móc edytować środek\n";
                }
                if (AnyRhombusAnalysis)
                {
                    ErrorMessage = ErrorMessage + "->Ten środek został uwzględniony w jednej z analiz romboidalnej .Usuń tą analizę aby " +
                                      "móc edytować środek\n";
                }
                return !(AnyRhombusAnalysis || AnyFrapAnalysis  || !IsFulfillAddingConditions);
            }
            else
            {
                bool IsEmptyMitagationList = !control.Mitagations.IsNullOrEmpty();
                if (IsEmptyMitagationList)
                {
                    ErrorMessage = ErrorMessage + "->Do tego środka zostały przypisane akcje.Usuń je albo oddznacz je w edycji środka aby " +
                        "móc usunąć środek\n";
                }
                Risk Firstrisk = new Risk();
                bool IsEmptyRisksList = false;
                if (control.Risks.Count == 1)
                {
                    Firstrisk = control.Risks.First();
                    if (Firstrisk.RiskId == SelectedRisk.RiskId)
                    {
                        IsEmptyRisksList = true;
                    }
                    else
                    {
                       
                        IsEmptyRisksList = false;

                    }
                }
                if (IsEmptyRisksList == false)
                {
                    ErrorMessage = ErrorMessage + "->Do tego środka zostały przypisane ryzyka.Usuń je albo oddznacz je w edycji środka aby " +
                                  "móc usunąć środek\n";
                }
                if (AnyFrapAnalysis)
                {
                    ErrorMessage = ErrorMessage + "->Ten środek został uwzględniony w jednej z analiz FRAP .Usuń tą analizę aby " +
                                      "móc usunąć środek\n";
                }
                if (AnyRhombusAnalysis)
                {
                    ErrorMessage = ErrorMessage + "->Ten środek został uwzględniony w jednej z analiz romboidalnej .Usuń tą analizę aby " +
                                      "móc usunąć środek\n";
                }
                return !(AnyRhombusAnalysis || AnyFrapAnalysis || !IsEmptyRisksList || IsEmptyMitagationList);

            }

        }
        public void DeleteControlRisk(ControlRisk control,Risk risk)
        {
            SelectedRisk = risk;
            if (CanDeleteOrEditControlRisk(control,false))
            {

                ControlRiskRepository.DeleteControlRisk(control);
                System.Windows.MessageBox.Show("Środek został pomyślnie usunięty", "Usuwanie Środka", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Środek  nie został pomyślnie usunięty ponieważ:\n" + ErrorMessage, "Usuwanie Środka", MessageBoxButton.OK,
                  MessageBoxImage.Error);
                ErrorMessage = null;
            }
        }
        private ControlRisk GetOriginalControlRiskAndChange(ControlRisk changedControl)
        {
            ControlRisk control = LoadControlRiskWithEntities(changedControl.ControlRiskId);
            control.Name = changedControl.Name;
            control.Description = changedControl.Description;
            control.Risks = changedControl.Risks;
            control.Mitagations = changedControl.Mitagations;
            
            return control;
        }
        public void EditControlRisk(ControlRisk Changedcontrol)
        {
            if (CanDeleteOrEditControlRisk(Changedcontrol,true))
            {
                ControlRisk control = GetOriginalControlRiskAndChange(Changedcontrol);
                ControlRiskRepository.EditControlRisk(control);
                System.Windows.MessageBox.Show("Środek został pomyślnie zmieniony", "Edycja Środka",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Środek  nie został pomyślnie zmieniony ponieważ:\n" + ErrorMessage, "Edycja Środka",MessageBoxButton.OK,
                    MessageBoxImage.Error);
                ErrorMessage = null;
            }
        } 
        public List<RiskDataGridItem> InitRisksForDataGrid(List<Risk> Risks,Asset asset)
        {
            List<RiskDataGridItem>RiskItems = new List<RiskDataGridItem>();
            foreach (Risk r in Risks)
            {
                Risk risk=LoadRiskWithEntities(r.RiskId);
                
                
                bool BelongToAssetOfSelectedAsset = asset.AssetId == risk.AssetId;
                RiskItems.Add(new RiskDataGridItem(risk.RiskId, risk.Name, risk.Description, risk.Asset.Name, BelongToAssetOfSelectedAsset));

            }
            return RiskItems;
        }
        public List<RiskDataGridItem>InitRisksForDataGrid(List<Risk> Risks, ControlRisk SelectedControlRisk)
        {
            List<RiskDataGridItem> RiskItems = new List<RiskDataGridItem>();
            foreach (Risk r in Risks)
            {
                Risk risk = LoadRiskWithEntities(r.RiskId);
                bool IsBelongRiskToControlRiskRisks = SelectedControlRisk.Risks.Contains(risk);
                RiskItems.Add(new RiskDataGridItem(risk.RiskId, risk.Name, risk.Description, risk.Asset.Name,IsBelongRiskToControlRiskRisks));

            }
            return RiskItems;
        
         }
        private MitagationAction LoadMitagationActionWithEntities(long id)
        {
            return MitigationActionService.LoadMitigationWithEntities(id);
        }

        public List<MitagationDataGridItem> InitMitigationForDataGrid(List<MitagationAction>mitagations)
        {
            List<MitagationDataGridItem>MitigationItems= new List<MitagationDataGridItem>();
            foreach (MitagationAction action in mitagations)
            {
                MitagationAction mit = LoadMitagationActionWithEntities(action.MitagatioActionId);
                MitigationItems.Add(new MitagationDataGridItem(mit.MitagatioActionId, mit.Action, mit.Person, mit.DateOfAction, false,mit.Status));
            }
            return MitigationItems;
        }
        public List<MitagationDataGridItem> InitMitigationForDataGrid(List<MitagationAction> mitagations,ControlRisk SelectedControlRisk)
        {
            List<MitagationDataGridItem> MitigationItems = new List<MitagationDataGridItem>();
            foreach (MitagationAction action in mitagations)
            {
                MitagationAction mit = LoadMitagationActionWithEntities(action.MitagatioActionId);
                bool HasBelongToSelectedControlRisk = SelectedControlRisk.Mitagations.Contains(action);
                MitigationItems.Add(new MitagationDataGridItem(mit.MitagatioActionId, mit.Action, mit.Person, mit.DateOfAction, HasBelongToSelectedControlRisk, mit.Status));
            }
            return MitigationItems;
        }
        public ControlRisk LoadControlRiskWithEntities(long id)
        {
            return ControlRiskRepository.LoadControlRiskWithEntities(id);
        }

    }
}
