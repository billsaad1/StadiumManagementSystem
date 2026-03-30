using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private User? _authenticatedUser;

        public event Action<User>? OnLoginSuccess;

        [RelayCommand]
        private void Login(object? parameter)
        {
            string password = "";
            if (parameter is System.Windows.Controls.PasswordBox passwordBox)
            {
                password = passwordBox.Password;
            }

            var user = App.Database.Authenticate(Username, password);
            if (user != null)
            {
                AuthenticatedUser = user;
                OnLoginSuccess?.Invoke(user);
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
        }
    }
}
