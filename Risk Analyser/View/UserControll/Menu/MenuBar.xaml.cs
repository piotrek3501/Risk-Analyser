using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Risk_analyser.View.UserControll.Menu
{
    /// <summary>
    /// Logika interakcji dla klasy MenuBar.xaml
    /// </summary>
    public partial class MenuBar : UserControl
    {
        private DataContext _context = new DataContext();
        public MenuBar()
        {
            InitializeComponent();
            MenuBarViewModel vm = new MenuBarViewModel(_context);
            DataContext = vm;
        }


  

     
    }
}
