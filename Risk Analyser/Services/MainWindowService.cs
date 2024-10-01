using Risk_analyser.Data;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Services
{
    public  class MainWindowService
    {
        public  static ControlRiskService ControlRiskService { get; set; }
        public static MitigationActionService mitigationActionService {  get; set; }
        public static AssetService AssetService { get; set; }
        public static RiskService RiskService { get; set; }
        public static FRAPAnalysisService FRAPAnalysisService { get; set; }
        public static RhombusAnalysisService RhombusAnalysisService { get; set; }
        private static ContextManager ContextManager { get; set; }
        private static AssetRepository AssetRepository { get; set; }
        private static ControlRiskRepository ControlRiskRepository { get; set; }
        private static FRAPAnalysisRepository FRAPAnalysisRepository { get; set; }
        private static MitagationRepository MitagationRepository { get; set; }
        private static RhombusAnalysisRepository RhombusAnalysisRepository { get; set; }
        private static RiskRepository RiskRepository { get; set; }
        private static FRAPDocumentRepository FRAPDocumentRepository { get; set; }
        private static RhombusDocumentRepository RhombusDocumentRepository {  get; set; } 
        public MainWindowService() {
            InitServices();
        }
        public static void  InitServices() {

            ContextManager = new ContextManager();
            AssetRepository = new AssetRepository(ContextManager.Assets);
            ControlRiskRepository = new ControlRiskRepository(ContextManager.ControlRisks);
            FRAPAnalysisRepository = new FRAPAnalysisRepository(ContextManager.FrapAnalysis);
            MitagationRepository = new MitagationRepository(ContextManager.mitagationActions);
            RhombusAnalysisRepository = new RhombusAnalysisRepository(ContextManager.RhombusAnalyses);
            RiskRepository = new RiskRepository(ContextManager.Risks);
            FRAPDocumentRepository= new FRAPDocumentRepository(ContextManager.FRAPDocuments);
            RhombusDocumentRepository=new RhombusDocumentRepository(ContextManager.rhombusDocuments);

            AssetService = new AssetService(AssetRepository);
            RiskService = new RiskService(RiskRepository);
            FRAPAnalysisService=new FRAPAnalysisService(FRAPAnalysisRepository,FRAPDocumentRepository);
            RhombusAnalysisService=new RhombusAnalysisService(RhombusAnalysisRepository,RhombusDocumentRepository);

            RiskService.SetRhombusAnalysisService();
            RiskService.SetFRAPAnalysisService();

            AssetService.SetFRAPAnalysisService();
            AssetService.SetRhombusAnalysisService();
            AssetService.SetRiskService();

            RhombusAnalysisService.SetAssetService();
            RhombusAnalysisService.SetRiskService();

            mitigationActionService = new MitigationActionService(MitagationRepository);
            mitigationActionService.SetAssetService();
            mitigationActionService.SetRiskService();
            mitigationActionService.SetFRAPAnalysisService();
            mitigationActionService.SetRhombusAnalysisService();

            ControlRiskService=new ControlRiskService(ControlRiskRepository);
            ControlRiskService.SetFRAPAnalysisService();
            ControlRiskService.SetRhombusAnalysisService();
            ControlRiskService.SetMitigationActionService();

            mitigationActionService.SetControlRiskService();


      
        }

        public static  RhombusAnalysisService GetRhombusAnalysisService()
        {
            return RhombusAnalysisService;
        }
        public static RiskService GetRiskService()
        {
            return RiskService;
        }
        public static AssetService GetAssetService()
        {
            return AssetService;
        }
        public static ControlRiskService GetControlRiskService()
        {
            return ControlRiskService;
        }
        public static MitigationActionService GetMitigationActionService()
        {
            return mitigationActionService;
        }
        public static FRAPAnalysisService GetFRAPAnalysisService()
        {
            return FRAPAnalysisService;
        }

        public static ObservableCollection<ControlRisk> LoadControlRiskForRisk(Risk selectedRisk)
        {
            return new ObservableCollection<ControlRisk>(ControlRiskService.GetAllControlRiskForRisk(selectedRisk));
        }
        public static ObservableCollection<MitagationAction> LoadMitigationForControlRisk(ControlRisk selectedControl)
        {
            return new ObservableCollection<MitagationAction>(mitigationActionService.GetMitagationsFromControlRisk(selectedControl));
        }
        public static ObservableCollection<Asset> LoadAllAssets()
        {
            return new ObservableCollection<Asset>(AssetService.GetAssets());
        }
        public static ObservableCollection<Risk> LoadAllRiskForAsset(Asset asset)
        {
            return new ObservableCollection<Risk>(RiskService.GetRisksForAsset(asset)); 
        }
    }
}
