using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Win32;
using OxyPlot;
using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using Risk_analyser.Services;
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
    public class ListCreatedFRAPsAnalysisViewModel : ViewModelBase
    {
        private Asset _selectedAsset { get; set; }
        private FRAPAnalysisService fRAPAnalysisService { get; set; }
        public string Title
        {
            get
            {
                return _selectedAsset.Name;
            }
            set
            {
            }
        }
        private ObservableCollection<FRAPAnalysisDataGridItem> _dataGridItems { get; set; }
        public ObservableCollection<FRAPAnalysisDataGridItem> RhombusAnalysisDataGrid
        {
            get
            {
                return _dataGridItems;
            }
            set
            {
                _dataGridItems = value;
                OnPropertyChanged(nameof(RhombusAnalysisDataGrid));
            }
        }
        public ICommand AnalysisLocalSavingCommand { get; set; }
        public ICommand DeleteAnalysisCommand { get; set; }
        public ListCreatedFRAPsAnalysisViewModel(Asset selectedAsset)
        {
            _selectedAsset = selectedAsset;
            fRAPAnalysisService = MainWindowService.FRAPAnalysisService;
            DeleteAnalysisCommand = new RelayCommand(parameter=>DeleteFRAPAnalyse(parameter),parameter=>parameter!=null);
            AnalysisLocalSavingCommand = new RelayCommand(parameter=>SaveToPDFLocally(parameter), parameter => parameter != null);
            RhombusAnalysisDataGrid=fRAPAnalysisService.GetFRAPDocuments(selectedAsset,AnalysisLocalSavingCommand,DeleteAnalysisCommand,true);
        }

        private void DeleteFRAPAnalyse(object parameter)
        {
            fRAPAnalysisService.DeleteFRAPAnalysis(((FRAPAnalysisDataGridItem)parameter).DocumentId);
            RhombusAnalysisDataGrid = fRAPAnalysisService.GetFRAPDocuments(_selectedAsset, AnalysisLocalSavingCommand, DeleteAnalysisCommand, true);
        }

        private void SaveToPDFLocally(object parameter)
        {
            fRAPAnalysisService.SaveLocalToPDF(((FRAPAnalysisDataGridItem)parameter).DocumentId);
        }

       
    }
}
