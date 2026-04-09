using System.Collections.Generic;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        private User? _currentUser;

        public event Action<User>? OnLoginSuccess;

        [RelayCommand]
        private void Login(object? parameter)
        {
            string password = "";
            if (parameter is System.Windows.Controls.PasswordBox passwordBox)
            {
                password = passwordBox.Password;
            }

            CurrentUser = App.Database.Authenticate(Username, password);
            if (CurrentUser != null)
            {
                OnLoginSuccess?.Invoke(CurrentUser);
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
        }
    }
}
