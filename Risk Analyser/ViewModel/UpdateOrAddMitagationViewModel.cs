using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using Risk_analyser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class UpdateOrAddMitagationViewModel :ViewModelBase
    {
        private MitigationActionService _MitigationActionService;
        private string _workerName;
        private string _action;
        private MitagationAction _selectedMitigation;
        private DateTime _realisationDate {  get; set; }
        private string _selectedAnswer;
        private List<string> _answerActionPerform;
        private ControlRisk _selectedControlRisk { get; set; }
        
        public List<string> AnswerActionPerform
        {

            get { return _answerActionPerform; }
            set {
                _answerActionPerform = value;
                OnPropertyChanged();
            }
            
        }
        public string SelectedAnswer
        {
            get { return _selectedAnswer; }
            set
            {
                _selectedAnswer = value;
                OnPropertyChanged();
            }
        }
        public DateTime RealisationDate
        {
            get
            {
                return _realisationDate;
            }
            set
            {
                _realisationDate = value;
                OnPropertyChanged();
            }
        }


        public MitagationAction SelectedMitigation
        {
            get { return _selectedMitigation; }
            set
            {
                _selectedMitigation = value;
                OnPropertyChanged();
            }
        }
        public bool DialogResult { get; private set; }

        public string WorkerName
        {
            get { return _workerName; }
            set
            {
                _workerName = value;
                OnPropertyChanged();
            }
        }

        public string Action
        {
            get { return _action; }
            set
            {
                _action = value;
                OnPropertyChanged();
            }
        }
        public ICommand CurrentCommand { get; set; }
        public ICommand CancelMitigationCommand { get; set; }
        public ICommand AddMitigationCommand { get; }
        public ICommand EditMitigationCommand { get; }
        public UpdateOrAddMitagationViewModel()
        {
            _MitigationActionService = MainWindowService.GetMitigationActionService();
            AddMitigationCommand = new RelayCommand(_ => AddMitigation(), _ => true);
            CancelMitigationCommand = new RelayCommand(_ => CloseWindow(), _ => true);
            RealisationDate = DateTime.Now;
            AnswerActionPerform = new List<string>();
            AnswerActionPerform.Add("TAK");
            AnswerActionPerform.Add("Nie");
            // IsEditMode = false;

        }
        public UpdateOrAddMitagationViewModel(MitagationAction mitigationAction) : this()
        {
            SelectedMitigation = mitigationAction;
            WorkerName = SelectedMitigation.Person;
            Action = SelectedMitigation.Action;
            EditMitigationCommand = new RelayCommand(_ => EditMitigation(), _ => true);
            CurrentCommand = EditMitigationCommand;
            // IsEditMode = true;

        }
        
        public UpdateOrAddMitagationViewModel(ControlRisk SelectedControl) : this()
        {
            _selectedControlRisk= SelectedControl;
            CurrentCommand = AddMitigationCommand;
        }

        private void EditMitigation()
        {
            //Asset asset = SelectedAsset;
            MitagationAction ChangedMitigation = new MitagationAction();
            ChangedMitigation.MitagatioActionId = SelectedMitigation.MitagatioActionId;
            ChangedMitigation.Action = Action;
            ChangedMitigation.Person = WorkerName;
            ChangedMitigation.DateOfAction = RealisationDate;
            //ChangedMitigation.Status = SelectedAnswer.Equals("TAK") ? true : false;
            ChangedMitigation.Status = RealisationDate <= DateTime.Now;

            DialogResult = _MitigationActionService.EditMitigation(ChangedMitigation);
            CloseWindow();
        }
        private void AddMitigation()
        {
            // Dodawanie nowego aktywa do bazy danych
            var newMitigation = new MitagationAction
            {
                Action = Action,
                Person = WorkerName,
                ControlRisks = new List<ControlRisk>(),
                DateOfAction = RealisationDate,
                CreationTime = DateTime.Now,
            };
            newMitigation.ControlRisks.Add(_selectedControlRisk);
            //newMitigation.Status= SelectedAnswer.Equals("TAK")?true:false;
            newMitigation.Status= RealisationDate <= DateTime.Now;
            DialogResult =_MitigationActionService.AddMitigationAction(newMitigation);

            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = DialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}

