using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Risk_analyser.Data;
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
    public class RiskService
    {
        private  RiskRepository _RiskRepository;
        private   FRAPAnalysisService _FRAPAnalysisService;
        private  RhombusAnalysisService _RhombusAnalysisService;
        private Risk _originalRisk;
        private string ErrorMessage {  get; set; }

        public RiskService(RiskRepository riskRepository)
        {
            _RiskRepository = riskRepository;
            //_FRAPAnalysisService = MainWindowService.GetFRAPAnalysisService();
            //_RhombusAnalysisService = MainWindowService.GetRhombusAnalysisService();
        }
        public void SetFRAPAnalysisService()
        {
            _FRAPAnalysisService = MainWindowService.GetFRAPAnalysisService();
        }
        public void SetRhombusAnalysisService()
        {
            _RhombusAnalysisService = MainWindowService.GetRhombusAnalysisService();
        }
        public List<Risk> GetRisksFromSelection(List<RiskDataGridItem> SelectedRisks)
        {
            List<Risk> Risks = new List<Risk>();

            foreach (RiskDataGridItem r in SelectedRisks)
            {
                if (r.BelongToControlRisk == true)
                {
                    Risks.Add(_RiskRepository.GetRiskById(r.RiskId));
                }
            }
            return Risks;
        }
        public bool AnyRiskForAsset(Asset asset)
        {
            return _RiskRepository.GetRisksForAsset(asset).Any();
        }
        public List<Risk> GetRisksForAsset(Asset asset) { 
            return _RiskRepository.GetRisksForAsset(asset);
        }
        private bool CanDeleteOrEditRisk(Risk risk,bool editing)
        {
            ErrorMessage = "";
            Asset asset = _RiskRepository.LoadRiskWithEntities(risk.RiskId).Asset;
            bool anyRhombus = _RhombusAnalysisService.OlderThanAllRhombusInAsset(asset,risk);
            bool anyFRAP = _FRAPAnalysisService.OlderThanAllFRAPsInAsset(asset, risk);

            if (editing)
            {
                bool IsFulfillAddingConditions=CanAddRisk(risk);
                if (anyFRAP)
                {
                    ErrorMessage = ErrorMessage + "->Te ryzyko zostało uwzględnione w jednej z analiz FRAP .Usuń tą analizę aby " +
                                      "móc edytować ryzyko\n";
                }
                if (anyRhombus)
                {
                    ErrorMessage = ErrorMessage + "->Te ryzyko zostało uwzględnione w jednej z analiz romboidalnej .Usuń tą analizę aby " +
                                   "móc edytować ryzyko\n";
                }

                
                return !(anyFRAP || anyRhombus  || !IsFulfillAddingConditions);

            }
            else
            {
                risk = _RiskRepository.LoadRiskWithEntities(risk.RiskId);
                bool anyControls = risk.Controls.Any();
                if (anyFRAP)
                {
                    ErrorMessage = ErrorMessage + "->Te ryzyko zostało uwzględnione w jednej z analiz FRAP .Usuń tą analizę aby " +
                                      "móc usunąć ryzyko\n";
                }
                if (anyRhombus)
                {
                    ErrorMessage = ErrorMessage + "->Te ryzyko zostało uwzględnione w jednej z analiz romboidalnej .Usuń tą analizę aby " +
                                   "móc usunąć ryzyko\n";
                }

                if (anyControls)
                {
                    ErrorMessage = ErrorMessage + "->Do tego ryzyka są przypisane Środki.Usuń te środki albo oddznacz te ryzyko w edycji poszczególnych środków aby " +
                                 "móc usunąć ryzyko\n";
                }
                return !(anyFRAP || anyRhombus || anyControls);

            }

        }
        public void DeleteRisk(Risk risk)
        {
            if (CanDeleteOrEditRisk(risk,false))
            {
                _RiskRepository.DeleteRisk(risk);
                System.Windows.MessageBox.Show("Ryzyko zostało pomyślnie usunięte", "Usuwanie Ryzyka", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Nie można usunąć ryzyka ponieważ:\n"+ErrorMessage, "Usuwanie Ryzyka", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorMessage = null;
            }

        }
        public List<Risk> GetRiskForControlRisk(long ControlRiskId)
        {
            return _RiskRepository.GetRisksForControlRisk(ControlRiskId);
        }
      
        public List<Risk> GetAllRiskWithAsset()
        {
            return _RiskRepository.GetAllRiskWithAsset();
        }
        public Risk LoadRiskWithEntities(long id)
        {
            return _RiskRepository.LoadRiskWithEntities(id);
        }
        public bool EditRisk(Risk ChangedRisk)
        {

            if (CanDeleteOrEditRisk(ChangedRisk,true))
            {
                 Risk risk=GetOriginalRiskAndChange(ChangedRisk);
                _RiskRepository.EditRisk(risk);
                System.Windows.MessageBox.Show("Ryzyko zostało pomyślnie zmienione", "Edycja Ryzyka", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                System.Windows.MessageBox.Show("Ryzyko nie zostało pomyślnie zmienione ponieważ:\n" + ErrorMessage, "Edycja Ryzyka", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorMessage = null;
                return false;

            }

        }

        private Risk GetOriginalRiskAndChange(Risk changedRisk)
        {
            Risk risk = LoadRiskWithEntities(changedRisk.RiskId);
            risk.Name = changedRisk.Name;
            risk.Description = changedRisk.Description;
            risk.RiskId = changedRisk.RiskId;
            risk.Vulnerability = changedRisk.Vulnerability;
            risk.CreationTime = changedRisk.CreationTime;
            risk.PotencialEffect = changedRisk.PotencialEffect;
            risk.RhombusParams = changedRisk.RhombusParams;
            risk.Priority = changedRisk.Priority;
            risk.Propability = changedRisk.Propability;
            risk.Types = changedRisk.Types;
            return risk;
        }

        public void AddRisk(Risk risk)
        {
           // risk=_RiskRepository.LoadRiskWithEntities(risk.RiskId);
            if (CanAddRisk(risk))
            {
                _RiskRepository.AddRisk(risk);
                System.Windows.MessageBox.Show("Ryzyko zostało pomyślnie dodane", "Utworzenie Ryzyka", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Ryzyko nie zostało pomyślnie dodane ponieważ:\n"+ErrorMessage, "Utworzenie Ryzyka", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorMessage = null;

            }
        }
        private bool CanAddRisk(Risk risk)
        {
            bool IsEmptyName = string.IsNullOrEmpty(risk.Name);
            if (IsEmptyName)
            {
                ErrorMessage = ErrorMessage + "->Nazwa nie może być pusta\n";
            }
            bool IsEmptyDescription=string.IsNullOrEmpty(risk.Description);
            if (IsEmptyDescription)
            {
                ErrorMessage = ErrorMessage + "->Opis nie może być pusty\n";

            }
            bool AnyRiskTypeSelected=risk.Types.Any();
            if (!AnyRiskTypeSelected)
            {
                ErrorMessage = ErrorMessage + "->Nie wybrano żadnego typu ryzyka \n";

            }

            return !(IsEmptyDescription ||IsEmptyName || !AnyRiskTypeSelected);
        }


    }
}
