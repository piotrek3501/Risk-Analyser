using DocumentFormat.OpenXml.Presentation;
using Risk_analyser.Data.Model;
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
using System.Windows.Forms;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class DeleteMitigationActionViewModel:ViewModelBase
    {
        public DeleteMitigationActionViewModel(MitagationDataGridItem mitagationActionDataGridItem)
        {
            _mitigationActionService = MainWindowService.GetMitigationActionService();
            _mitagationAction = _mitigationActionService.LoadMitigationWithEntities(mitagationActionDataGridItem.IdMitagation);
            DeleteMitigationCommand = new RelayCommand(_ => DeleteMitigation(), _ => true);
            CancelMitigationCommand = new RelayCommand(_ => CloseWindow(), _ => true);

        }
        public DeleteMitigationActionViewModel(MitagationAction mitagationAction,ControlRisk SelectedcontrolRisk)
        {
            _mitigationActionService = MainWindowService.GetMitigationActionService();
            _mitigationActionService.SetSelectedControlRisk(SelectedcontrolRisk);
            _mitagationAction = _mitigationActionService.LoadMitigationWithEntities(mitagationAction.MitagatioActionId);
            DeleteMitigationCommand = new RelayCommand(_ => DeleteMitigation(), _ => true);
            CancelMitigationCommand = new RelayCommand(_ => CloseWindow(), _ => true);

        }

        private MitagationAction _mitagationAction { get; }
        private MitigationActionService _mitigationActionService { get; }
        public bool DialogResult { get; set; }
        public ICommand DeleteMitigationCommand { get; }
        public ICommand CancelMitigationCommand { get; }
        public string MitigationWithExtraText
        {
            get
            {
                return _mitagationAction.Action + " ?";
            }

        }
        private void DeleteMitigation()
        {
            DialogResult= _mitigationActionService.DeleteMitigation(_mitagationAction);
            CloseWindow();

        }

        private void CloseWindow()
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
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

