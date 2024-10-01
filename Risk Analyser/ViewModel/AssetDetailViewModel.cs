using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.Services;
using Risk_analyser.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Risk_analyser.ViewModel
{
    public class AssetDetailViewModel : ViewModelBase
    {
        private string _name { get; set; }
        private Asset _selectedAsset { get; set; }
        private string _description { get; set; }
        private DateTime _assetDate { get; set; }
        public ICommand ShowFRAPsRaportsCommand { get; set; }
        public ICommand ShowRhombusRaportsCommand { get; set; }
        public ICommand ShowDialogueMakingAnalysis { get; set; }
        private RhombusAnalysisService RhombusService {  get; set; }
        private AssetService _assetService { get; set; }
        public string AssetName
        {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }
        public string AssetDescription
        {
            get {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
       
        public string AssetCreationDate
        {
            get{
                return _assetDate.ToString("dd-MM-yyyy");
            }
            set { }
           
        }
        public AssetDetailViewModel(Asset SelectedAsset)
        {
            _selectedAsset = SelectedAsset;
            _assetService = MainWindowService.GetAssetService();
            AssetName = SelectedAsset.Name;
            ShowFRAPsRaportsCommand=new RelayCommand(_=>ShowFRAPsRaports(),_=>true);
            ShowRhombusRaportsCommand=new RelayCommand(_=>ShowRhombusRaports(),_=>true);
            ShowDialogueMakingAnalysis =new RelayCommand(_=>MakeAnalysis(),_=>true);

            AssetDescription = SelectedAsset.Description;
            _assetDate = SelectedAsset.CreationTime;
        }
        private void ShowFRAPsRaports()
        {
            ListCreatedFRAPsAnalysisView listCreatedFRAPsAnalysisView = new ListCreatedFRAPsAnalysisView();
            ListCreatedFRAPsAnalysisViewModel listCreatedFRAPsAnalysisViewModel = new ListCreatedFRAPsAnalysisViewModel(_selectedAsset);
            listCreatedFRAPsAnalysisView.DataContext= listCreatedFRAPsAnalysisViewModel;
            listCreatedFRAPsAnalysisView.ShowDialog();
        }
        private void ShowRhombusRaports()
        {
            CreatedRhombusAnalysisView createdRhombusAnalysisView=new CreatedRhombusAnalysisView();
            CreatedRhombusAnalysisViewModel createdRhombusAnalysisViewModel=new CreatedRhombusAnalysisViewModel(_selectedAsset);
            createdRhombusAnalysisView.DataContext= createdRhombusAnalysisViewModel;
            createdRhombusAnalysisView.ShowDialog();
        }
        private void MakeAnalysis()
        {
            if(MessageBox.Show("Czy przeprowadzić analizę FRAP ?","Analiza FRAP", MessageBoxButton.YesNo, MessageBoxImage.Question)
                ==MessageBoxResult.Yes)
            {
                FRAPAnalysisService fRAPAnalysisService=MainWindowService.FRAPAnalysisService;
                _selectedAsset = _assetService.LoadAssetWithEntities(_selectedAsset.AssetId);
                FRAPDocument frapAnalyse= fRAPAnalysisService.MakeAnalyse(_selectedAsset);
                if (MessageBox.Show("Czy zapisać analizę FRAP lokalnie do pliku docx ?", "Zapis FRAP do docx", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
                {
                    SaveFileDialog saveFileDialog=new SaveFileDialog();
                    saveFileDialog.DefaultExt = ".docx";
                    saveFileDialog.Filter = "Word Document (*.docx)|*.docx|All files (*.*)|*.*";
                    string path = null;
                    if (saveFileDialog.ShowDialog()==true)
                    {
                        path = saveFileDialog.FileName;
                        fRAPAnalysisService.SaveAnalyseLocalyToDocsFile(frapAnalyse.DocumentId,path);
                    }
                }
            }
            if(MessageBox.Show("Czy przeprowadzić analizę Romboidalna ?", "Analiza Romboidalna", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes){
                RhombusAnalysisView rhombusviewer= new RhombusAnalysisView();
                _selectedAsset = _assetService.LoadAssetWithEntities(_selectedAsset.AssetId);
                rhombusviewer.DataContext = new RhombusAnalysisViewerViewModel(_selectedAsset);
                rhombusviewer.ShowDialog();
            }
        }
    }
}

