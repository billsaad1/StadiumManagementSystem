using System.Windows;
using StadiumManagementSystem.ViewModels;

namespace StadiumManagementSystem.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            var vm = new LoginViewModel();
            DataContext = vm;
            vm.OnLoginSuccess += (user) => 
            {
                DialogResult = true;
                Close();
            };
        }
    }
}
