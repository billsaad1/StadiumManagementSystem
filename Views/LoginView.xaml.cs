using StadiumManagementSystem.ViewModels;
using StadiumManagementSystem.Models;
using System.Windows;

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
