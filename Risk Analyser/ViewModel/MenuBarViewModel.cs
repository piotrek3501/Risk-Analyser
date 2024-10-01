using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Risk_analyser.ViewModel
{
    public class MenuBarViewModel : ViewModelBase
    {
        public DataContext Context { get; }
        public ICommand AddAssetCommand { get; set; }

        public MenuBarViewModel(DataContext context)
        {
            Context = context;
            //AddAssetCommand=((MainWindowViewModel)DataContext).AddAssetCommand;
        }

    }
}
