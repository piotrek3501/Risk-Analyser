using Risk_analyser.Data;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Risk_analyser.Services
{
    public class AssetService
    {
        private  AssetRepository _assetRepository;
        private  FRAPAnalysisService _FRAPAnalysisService;
        private  RhombusAnalysisService _rhombusAnalysisService;
        private  RiskService _RiskService;
        private string ErrorMessage {  get; set; }
       

        public AssetService(AssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
            //_FRAPAnalysisService = MainWindowService.FRAPAnalysisService;
            //_rhombusAnalysisService = MainWindowService.RhombusAnalysisService;
            //_RiskService = MainWindowService.GetRiskService();
        }
        public void SetFRAPAnalysisService()
        {
            _FRAPAnalysisService = MainWindowService.GetFRAPAnalysisService();
        }
        public void SetRhombusAnalysisService()
        {
            _rhombusAnalysisService = MainWindowService.GetRhombusAnalysisService();
        }
        public void SetRiskService()
        {
            _RiskService= MainWindowService.GetRiskService();
        }
        public List<Asset> GetAssets()
        {
            return _assetRepository.GetAllAssets();
        }
        public Asset LoadAssetWithEntities(long id)
        {
            return _assetRepository.LoadAssetWithEntities(id);
        }

        public bool CanDeleteAsset(Asset asset)
        {
            ErrorMessage = "";
            bool anyFRAP = _FRAPAnalysisService.AnyFRAPAnalysis(asset);
            if (anyFRAP) {
                ErrorMessage =ErrorMessage+ "->Dla tego zasobu istnieją wygenerowane analizy FRAP..Usuń je aby móc usunąć zasób\n";
            }
            bool anyRhombus = _rhombusAnalysisService.AnyRhombusAnalysis(asset);
            if (anyRhombus) {
                ErrorMessage = ErrorMessage+ "->Dla tego zasobu istnieją wygenerowane analizy Romboidalne.Usuń je aby móc usunąć zasób\n";

            }
            bool anyRisks = _RiskService.AnyRiskForAsset(asset);
            if (anyRisks) {
                ErrorMessage = ErrorMessage + "->Dla tego zasobu zostały dodane Ryzyka.Usuń je aby móc usunąć zasób\n";
            }
          

            return !(anyFRAP || anyRhombus || anyRisks);
        }
        public void DeleteAsset(Asset asset)
        {
            if (CanDeleteAsset(asset)) { 
                _assetRepository.DeleteAsset(asset);
                System.Windows.MessageBox.Show("Zasób został pomyślnie usunięty", "Usuwanie Zasobu", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Nie można usunąć zasobu ponieważ:\n"+ErrorMessage ,"Usuwanie Zasobu", MessageBoxButton.OK,
                   MessageBoxImage.Error);
                ErrorMessage = null;

            }
        }
        private bool CanAddOrUpdateAsset(Asset asset)
        {
            ErrorMessage = "";
            bool IsEmptyName = string.IsNullOrWhiteSpace(asset.Name);
            bool IsEmptyDescription = string.IsNullOrWhiteSpace(asset.Description);
            if (IsEmptyName) {
                ErrorMessage = ErrorMessage+ "->Nazwa zasobu nie może być pusta\n";
            }
            if (IsEmptyDescription) {
                ErrorMessage = ErrorMessage + "->Opis zasobu nie może być pusty\n";
            }

            return !IsEmptyName && !IsEmptyDescription;

        }
        public void EditAsset(Asset ChangedAsset)
        {
            if (CanAddOrUpdateAsset(ChangedAsset))
            {
                Asset asset=GetOriginalAssetAndChange(ChangedAsset);
                _assetRepository.EditAsset(asset);
            }
            else
            {
                MessageBox.Show("Zasób nie został pomyślnie zmieniony ponieważ:\n " + ErrorMessage, "Edycja zasobu", MessageBoxButton.OK, MessageBoxImage.Error);
                ErrorMessage = null;

            }
        }
        private Asset GetOriginalAssetAndChange(Asset changedAsset)
        {
            Asset asset = _assetRepository.LoadAssetWithEntities(changedAsset.AssetId);
            asset.Name = changedAsset.Name;
            asset.Description = changedAsset.Description;
           
            return asset;
        }
        public void AddAsset(Asset asset)
        {
            if (CanAddOrUpdateAsset(asset))
            {
                _assetRepository.AddAsset(asset);
                MessageBox.Show("Zasób został pomyślnie dodany", "Zasób dodany",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Zasób nie został pomyślnie dodany ponieważ:\n "+ErrorMessage, "Tworzenie zasobu",MessageBoxButton.OK,MessageBoxImage.Error);
                ErrorMessage = null;

            }
        }
      
    }
}
