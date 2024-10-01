using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class CreatedRhombusAnalysisViewModel:ViewModelBase
    {
        private Asset _selectedAsset { get; set; }
        private RhombusAnalysisService _rhombusAnalysisService { get; set; }
        public string Title {
            get
            {
                return _selectedAsset.Name;
            }
            set {
            }
        }
        private ObservableCollection<RhombusAnalysisDataGridItem> _dataGridItems {  get; set; }
        public ObservableCollection<RhombusAnalysisDataGridItem> RhombusAnalysisDataGrid 
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
        public ICommand AnalysisLocalSavingCommand {  get; set; }
        public ICommand DeleteAnalysisCommand {  get; set; }

        public CreatedRhombusAnalysisViewModel(Asset asset)
        {
            _selectedAsset = asset;
            AnalysisLocalSavingCommand = new RelayCommand(parameter => SaveAnalysisLocally(parameter),parameter=>parameter!=null);
            DeleteAnalysisCommand=new RelayCommand(parameter=>DeleteAnalysis(parameter),parameter=>parameter!=null);
            _rhombusAnalysisService=MainWindowService.GetRhombusAnalysisService();
            RhombusAnalysisDataGrid = _rhombusAnalysisService.GetRhombusDocuments(_selectedAsset,AnalysisLocalSavingCommand,DeleteAnalysisCommand,true);
        }

        private void DeleteAnalysis(object parameter)
        {
            _rhombusAnalysisService.DeleteRhombusAnalysis(((RhombusAnalysisDataGridItem)parameter).DocumentId);
            RhombusAnalysisDataGrid = _rhombusAnalysisService.GetRhombusDocuments(_selectedAsset, AnalysisLocalSavingCommand, DeleteAnalysisCommand, true);
            OnPropertyChanged();
        }

        private void SaveAnalysisLocally(object parameter)
        {
            _rhombusAnalysisService.SaveLocalToPDF(((RhombusAnalysisDataGridItem)parameter).DocumentId);

        }
    }
}
