using Risk_analyser.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Risk_analyser.Data.Model
{
    public class FRAPAnalysisDataGridItem :ViewModelBase
    {
        public long DocumentId { get; set; }
        public string FileName { get; set; }
        public string CreationTime { get; set; }
        private ICommand _saveLocally { get; set; }
        private ICommand _deleteAnalyse { get; set; }
        public ICommand SaveLocally
        {
            get
            {
                return _saveLocally;
            }
            set
            {
                _saveLocally = value;
                OnPropertyChanged(nameof(SaveLocally));
            }
        }
        public ICommand DeleteAnalyse
        {
            get
            {
                return _deleteAnalyse;
            }
            set
            {
                _deleteAnalyse = value;
                OnPropertyChanged(nameof(DeleteAnalyse));
            }
        }
    }
}
