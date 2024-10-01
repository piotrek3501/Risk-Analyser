using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
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
        //private  DataContext _context=new DataContext();
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm=new MainWindowViewModel();
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
        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                // Uzyskaj element, który został kliknięty
                var selectedAsset = listBoxItem.DataContext as Asset; // Zastąp YourAssetType odpowiednim typem

                // Uzyskaj odniesienie do okna
                var window = Window.GetWindow(listBoxItem);

                // Sprawdź, czy DataContext jest odpowiedniego typu
                if (window?.DataContext is MainWindowViewModel viewModel)
                {
                    // Sprawdź, do której listy należy element
                    var listBox = FindParentListBox(listBoxItem);
                    if (listBox != null)
                    {
                        if (listBox.Name == "AssetsList")
                        {
                            // Wywołaj polecenie dla AssetsList
                            if (viewModel.AssetListDoubleClickCommand.CanExecute(selectedAsset))
                            {
                                viewModel.AssetListDoubleClickCommand.Execute(selectedAsset);
                            }
                        }
                        // Dodaj inne warunki dla innych ListBoxów, jeśli to konieczne
                        else if (listBox.Name == "RisksList")
                        {
                            if (viewModel.RiskListDoubleClickCommand.CanExecute(null))
                            {
                                viewModel.RiskListDoubleClickCommand.Execute(null);
                            }
                        }
                    }
                }
            }
        }
        private ListBox FindParentListBox(DependencyObject child)
        {
            while (child != null)
            {
                if (child is ListBox listBox)
                    return listBox;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }


    }
}