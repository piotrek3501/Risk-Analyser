using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Risk_analyser.Data.DBContext;

using System.Windows;
using Microsoft.Extensions.Hosting;
using Risk_analyser.MVVM;
using Risk_analyser.ViewModel;
using Risk_analyser.Data.Repository;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data;
using Risk_analyser.Services;
using Risk_analyser.services;
using System.Text;


namespace Risk_analyser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}


