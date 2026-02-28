using System.Windows;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Views;
using StadiumManagementSystem.ViewModels;

namespace StadiumManagementSystem
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; } = null!;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Database = new DatabaseService();

            // Load theme from settings
            var settings = Database.GetSettings();
            Helpers.ThemeHelper.ApplyTheme(settings.ThemeColor);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var loginView = new LoginView();
            if (loginView.ShowDialog() == true)
            {
                var vm = (LoginViewModel)loginView.DataContext;
                var mainWindow = new MainWindow();
                mainWindow.DataContext = new MainViewModel(vm.CurrentUser!);

                ShutdownMode = ShutdownMode.OnLastWindowClose;
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
