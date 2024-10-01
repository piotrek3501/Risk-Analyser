using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using Risk_analyser.Services;
using Risk_analyser.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Risk_analyser.ViewModel
{
    public class ControlRiskDetailsViewModel : ViewModelBase
    {
        private ControlRiskService _controlRiskService;
        private MitigationActionService _mitigationActionService { get; set; }
        private string _controlRiskName {  get; set; }
        private string _controlRiskDescription { get; set; }
        private DateTime _controlRiskCreationDate {  get; set; }
        private ObservableCollection<MitagationDataGridItem> _controlRiskMitagationList { get; set; }
        private ControlRisk _selectedControlRisk { get; set; }
        public string ControlRiskName
        {
            get { return _controlRiskName; }
            set { }
        }
        public string ControlRiskDescription
        {
            get { return _controlRiskDescription; }
            set { }
        }
        public string ControlRiskCreationDate
        {
            get { return _controlRiskCreationDate.ToString("dd-MM-yyyy"); }
            set { }
        }
        public ObservableCollection<MitagationDataGridItem> ControlRiskMitagationList
        {
            get { return _controlRiskMitagationList; }
            set {
                _controlRiskMitagationList= value;
                OnPropertyChanged(nameof(ControlRiskMitagationList));
            }
          
        }
        public ICommand AddMitigationCommand {  get; set; }
        

        public ControlRiskDetailsViewModel(ControlRisk selectedControlRisk)
        {
            _controlRiskService = MainWindowService.GetControlRiskService();
            _selectedControlRisk = _controlRiskService.LoadControlRiskWithEntities(selectedControlRisk.ControlRiskId);
            _mitigationActionService=MainWindowService.GetMitigationActionService();
            _controlRiskName =_selectedControlRisk.Name;
            _controlRiskDescription=_selectedControlRisk.Description;
            _controlRiskCreationDate=_selectedControlRisk.CreationDate;
            _mitigationActionService.SetSelectedControlRisk(_selectedControlRisk);

            AddMitigationCommand=new RelayCommand(_=> ShowUpdateOrAddMitigationWindow(null,false), _=> true);
            LoadMitagations();



        }

        private void LoadMitagations()
        {
            if (_selectedControlRisk != null)
            {
                //Mitagations = MainWindowService.LoadMitigationForControlRisk(SelectedControlRisk);
                _controlRiskMitagationList = new ObservableCollection<MitagationDataGridItem>();
                List<MitagationAction> mitagationFromControlRisk = _mitigationActionService.GetMitagationsFromControlRisk(_selectedControlRisk);
                foreach (MitagationAction mit in mitagationFromControlRisk)
                {
                    MitagationDataGridItem mitAction = new MitagationDataGridItem(mit.MitagatioActionId, mit.Action, mit.Person, mit.DateOfAction
                        ,true, mit.Status);

                   // mitAction.Commands.Add(new OperationItem { Name = "Edytuj", Command = new RelayCommand(param => ShowUpdateOrAddMitigationWindow(mitAction, true),_=>true) });
                    //mitAction.Commands.Add(new OperationItem { Name = "Usuń", Command = new RelayCommand(param => DeleteMitagation(mitAction),_=>true) });
                    mitAction.EditAction = new RelayCommand(param => ShowUpdateOrAddMitigationWindow(mitAction, true), _ => true);
                    mitAction.DeleteAction= new RelayCommand(param => ShowDeletingMitagationWindow(mitAction), _ => true);

                    _controlRiskMitagationList.Add(mitAction);
                }
            }
            OnPropertyChanged(nameof(ControlRiskMitagationList));


        }

        private void ShowDeletingMitagationWindow(MitagationDataGridItem mit)
        {
            var deleteMitigationActionView = new DeleteMitigationActionView();
            deleteMitigationActionView.DataContext = new DeleteMitigationActionViewModel(mit);
            deleteMitigationActionView.ShowDialog();
            if (deleteMitigationActionView.DialogResult == true)
            {
                OnPropertyChanged(nameof(MitagationAction));
            }
            //_selectedControlRisk = null;
            LoadMitagations();
            OnPropertyChanged(nameof(ControlRiskMitagationList));
            OnPropertyChanged();
        }

      
        public  void ShowUpdateOrAddMitigationWindow(MitagationDataGridItem ?SelectedMitigationDataGridItem, bool Editing)
        {
            var updateOrAddMitagtionActionView = new UpdateOrAddMitigationActionView();
            UpdateOrAddMitagationViewModel updateOrAddMitagationViewModel;
            if (Editing)
            {
               // IsEditMode = true;
                MitagationAction SelectedMitigation=_mitigationActionService.LoadMitigationWithEntities(SelectedMitigationDataGridItem.IdMitagation);
                updateOrAddMitagationViewModel = new UpdateOrAddMitagationViewModel(SelectedMitigation);
                updateOrAddMitagtionActionView.DataContext = updateOrAddMitagationViewModel;
            }
            else
            {
                //IsEditMode = false;
                
                updateOrAddMitagationViewModel = new UpdateOrAddMitagationViewModel(_selectedControlRisk);
                updateOrAddMitagtionActionView.DataContext = updateOrAddMitagationViewModel;
            }

            updateOrAddMitagtionActionView.ShowDialog();
            if (updateOrAddMitagationViewModel.DialogResult == true)
            {
                OnPropertyChanged(nameof(MitagationAction));
            }
            //_selectedControlRisk = null;
            LoadMitagations();
            OnPropertyChanged(nameof(ControlRiskMitagationList));
            OnPropertyChanged();
        }
    }
}
