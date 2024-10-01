using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.Services;
using Risk_analyser.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using Risk_analyser.services;

namespace Risk_analyser.ViewModel
{
    public class MitigationActionDetailsViewModel :ViewModelBase
    {
        private string _name { get; set; }
        private MitagationAction _selectedMitigation { get; set; }
        private string _worker { get; set; }
        private DateTime _mitigationDate { get; set; }
        private DateTime _mitigationRealisationDate { get; set; }
        private bool _status {  get;  }
        private ObservableCollection<ControlRisk> _controls { get; set; }
        private MitigationActionService _mitigationService { get; set; }


        public string MitigationName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string MitigationWorker
        {
            get
            {
                return _worker;
            }
            set
            {
                _worker = value;
            }
        }

        public string MitigationCreationDate
        {
            get
            {
                return _mitigationDate.ToString("dd-MM-yyyy");
            }
            set { }

        }
        public string MitigationRealisationDate
        {
            get
            {
                return _mitigationRealisationDate.ToString("dd-MM-yyyy");
            }
            set { }

        }
        public string MitigationStatus
        {
            get
            {
                if (_selectedMitigation.Status)
                {
                    return "TAK";
                }
                return "NIE";
            }
            set { }
        }
        public ObservableCollection<ControlRisk> Controls {
            get
            {
                return _controls;
            }
            set
            {
                _controls = value;
                OnPropertyChanged();
            }
        }

        public MitigationActionDetailsViewModel(MitagationAction SelectedMitigation)
        {
            _mitigationService=MainWindowService.mitigationActionService;
            _selectedMitigation = _mitigationService.LoadMitigationWithEntities(SelectedMitigation.MitagatioActionId);
            _name = _selectedMitigation.Action;
            _worker = _selectedMitigation.Person;
            _mitigationDate = _selectedMitigation.CreationTime;
            _mitigationRealisationDate= _selectedMitigation.DateOfAction;
            _status= _selectedMitigation.Status;
            _controls=new ObservableCollection<ControlRisk>(_selectedMitigation.ControlRisks);
        }
        
        
    }
}

