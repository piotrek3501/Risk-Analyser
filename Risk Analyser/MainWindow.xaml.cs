using Risk_analyser.Data.DBContext;
using Risk_analyser.MVVM;
using Risk_analyser.ViewModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Risk_analyser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private  DataContext _context=new DataContext();
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm=new MainWindowViewModel(_context);
            DataContext = vm;

        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Shutdown();
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState=WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState=WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

       
    }
}