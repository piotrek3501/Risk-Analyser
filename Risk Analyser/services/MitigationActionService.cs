using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Risk_analyser.services
{
    public class MitigationActionService
    {
        private  MitagationRepository _MitagationRepository {  get; set; }
        private  ControlRiskService _ControlRiskService {  get;  set; }
        private RiskService _RiskService { get; set; }
        private AssetService _AssetService { get; set; }
        private FRAPAnalysisService _FRAPAnalysisService { get; set; }
        private RhombusAnalysisService _rhombusAnalysisService {  get; set; }

        private ControlRisk SelectedControlRisk { get; set; }
        private string ErrorMessage { get; set; }

        public MitigationActionService(MitagationRepository mitagationRepository)
        {
            _MitagationRepository = mitagationRepository;
            //_FRAPAnalysisService=MainWindowService.GetFRAPAnalysisService();
            //_rhombusAnalysisService=MainWindowService.GetRhombusAnalysisService();
        }
        public void SetControlRiskService()
        {
            this._ControlRiskService= MainWindowService.GetControlRiskService();
        }
        public void SetFRAPAnalysisService()
        {
            _FRAPAnalysisService = MainWindowService.GetFRAPAnalysisService();
        }
        public void SetRhombusAnalysisService()
        {
            _rhombusAnalysisService = MainWindowService.GetRhombusAnalysisService();
        }
        public void SetAssetService()
        {
            _AssetService= MainWindowService.GetAssetService();
        }
        public void SetRiskService()
        {
            _RiskService= MainWindowService.GetRiskService();
        }
        public void SetSelectedControlRisk(ControlRisk selectedcontrol)
        {
            SelectedControlRisk = selectedcontrol;
        }

        public List<MitagationAction> GetMitgationFromSelection(List<MitagationDataGridItem> SelectedMitagation)
        {
            List<MitagationAction> mitagationActions = new List<MitagationAction>();
            foreach (MitagationDataGridItem m in SelectedMitagation)
            {
                if (m.BelongToControlRisk == true)
                {
                    mitagationActions.Add(_MitagationRepository.GetMitagationFromId(m.IdMitagation));
                }
            }
            return mitagationActions;
        }
        public List<MitagationAction> GetMitagationsFromControlRisk(ControlRisk controlRisk)
        {
            return _MitagationRepository.GetAllMitigationFromControlRisk(controlRisk);
        }
        public List<MitagationAction> GetAllMitigation()
        {
            return _MitagationRepository.GetAllMitigation();
        }
        public MitagationAction LoadMitigationWithEntities(long id)
        {
            return _MitagationRepository.LoadMitagationWithEntities(id);
        }
       
        private bool CanDeleteOrEditMitigation(MitagationAction SelectedMitigation,bool editing)
        {
            ErrorMessage = "";

            List<ControlRisk> Controls=_MitagationRepository.LoadMitagationWithEntities(SelectedMitigation.MitagatioActionId).ControlRisks;
            List<Risk> RisksForControl = new List<Risk>();
            List<Asset> AssetsForControl = new List<Asset>();
            foreach (ControlRisk ControlRisk in Controls)
            {
                
                foreach(Risk risk in _ControlRiskService.GetRisksForControlRisk(ControlRisk.ControlRiskId))
                {
                    if (!RisksForControl.Contains(risk))
                    {
                        RisksForControl.Add(risk);
                    }
                    if (!AssetsForControl.Any(x => x.AssetId == risk.Asset.AssetId))
                    {
                        AssetsForControl.Add(risk.Asset);

                    }

                }
             
             }
            
            bool anyFRAP=true;
            bool anyRhombus= true;
            foreach (Asset asset in AssetsForControl)
            {
                anyFRAP = _FRAPAnalysisService.OlderThanAllFRAPsInAsset(asset, SelectedMitigation);
                anyRhombus = _rhombusAnalysisService.OlderThanAllRhombusInAsset(asset, SelectedMitigation);
                if (anyFRAP)
                {
                    ErrorMessage = ErrorMessage + "->Ta akcja została uwzgledniona w jednej z analiz FRAP.Usuń tą analizę aby móc usunąć akcje łagodzącą\n";
                }
                if (anyRhombus)
                {
                    ErrorMessage = ErrorMessage + "->Ta akcja została uwzgledniona w jednej z analiz romboidalnej.Usuń tą analizę aby móc usunąć akcje łagodzącą\n";
                }
                if (anyFRAP || anyRhombus)
                {
                    break;
                }

            }
            bool anyControlRisks = AnyControlRisk(SelectedMitigation);
            if (anyControlRisks)
            {
                ErrorMessage = ErrorMessage + "->Ta akcja jest powiązana z innymi środkami zaradczymi.Aby usuniąć akcje ,usuń tę środki lub odłacz je od tej akcji \n";
            }
            if (editing)
            {
                bool IsEmptyAction = string.IsNullOrWhiteSpace(SelectedMitigation.Action);
                bool IsEmptyPerson = string.IsNullOrWhiteSpace(SelectedMitigation.Person);
                if (IsEmptyAction)
                {
                    ErrorMessage = ErrorMessage + "->Opis akcji nie może być pusta\n";
                }
                if (IsEmptyPerson)
                {
                    ErrorMessage = ErrorMessage + "->Imie i nazwisko pracownika nie może być pusty\n";
                }
                return (!IsEmptyAction || !IsEmptyPerson) && (!(anyControlRisks || anyFRAP || anyRhombus));
            }


            return !(anyControlRisks||anyFRAP||anyRhombus);
        }
        private bool AnyControlRisk(MitagationAction SelectedMitigation)
        {
            SelectedMitigation=LoadMitigationWithEntities(SelectedMitigation.MitagatioActionId);
            return !(SelectedMitigation.ControlRisks.Any(x=>x.ControlRiskId==SelectedControlRisk.ControlRiskId) && 
                SelectedMitigation.ControlRisks.Count==1);
        }
        public bool DeleteMitigation(MitagationAction SelectedMitigation)
        {
            if (CanDeleteOrEditMitigation(SelectedMitigation,false))
            {
                _MitagationRepository.DeleteMitagationAction(SelectedMitigation);
                System.Windows.MessageBox.Show("Akcja został pomyślnie usunięty", "Usuwanie akcji", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                System.Windows.MessageBox.Show("Nie można usunąć akcji ponieważ:\n" + ErrorMessage, "Usuwanie akcji", MessageBoxButton.OK,
                   MessageBoxImage.Error);
                ErrorMessage = null;
            }
            return false;

        }
        private bool CanAddMitigationAction(MitagationAction Mitigation,bool editing)
        {
            ErrorMessage = "";
            bool IsEmptyAction = string.IsNullOrWhiteSpace(Mitigation.Action);
            bool IsEmptyPerson = string.IsNullOrWhiteSpace(Mitigation.Person);
            if (IsEmptyAction)
            {
                ErrorMessage = ErrorMessage + "->Opis akcji nie może być pusta\n";
            }
            if (IsEmptyPerson)
            {
                ErrorMessage = ErrorMessage + "->Imie i nazwisko pracownika nie może być pusty\n";
            }
            if (editing)
            {

            }

            return !IsEmptyAction || !IsEmptyPerson;

        }
        public bool EditMitigation(MitagationAction ChangedMitigation)
        {

            if (CanDeleteOrEditMitigation(ChangedMitigation,true))
            {
                MitagationAction action = GetOriginalMitigationAndChange(ChangedMitigation);
                _MitagationRepository.EditMitagationAction(action);
                return true;
            }
            else
            {
                MessageBox.Show("Akcja nie została pomyślnie zmieniona ponieważ:\n " + ErrorMessage, "Edycja Akcji", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorMessage = null;

            }
            return false;

        }
        private MitagationAction GetOriginalMitigationAndChange(MitagationAction ChangedMitigation)
        {
            MitagationAction action = _MitagationRepository.LoadMitagationWithEntities(ChangedMitigation.MitagatioActionId);
            action.Action = ChangedMitigation.Action;
            action.Person = ChangedMitigation.Person;
            action.Status = ChangedMitigation.Status;
            action.DateOfAction = ChangedMitigation.DateOfAction;

            return action;
        }
        public bool AddMitigationAction(MitagationAction newMitigation)
        {
            if (CanAddMitigationAction(newMitigation,false))
            {
                _MitagationRepository.AddMitagationAction(newMitigation);
                MessageBox.Show("Akcja został pomyślnie dodana", "Akcja dodana",MessageBoxButton.OK,MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show("Akca nie został pomyślnie dodana ponieważ:\n " + ErrorMessage, "Tworzenie Akcji", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorMessage = null;

            }
            return false;
        }
    }
}
