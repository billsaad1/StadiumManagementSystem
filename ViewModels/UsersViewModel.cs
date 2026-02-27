using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<User> _users;

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _role = "Cashier";

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private string _notes = string.Empty;

        public List<string> Roles { get; } = new() { "Admin", "Cashier", "Viewer" };

        public UsersViewModel()
        {
            _users = new ObservableCollection<User>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<User>(App.Database.GetUsers());
        }

        [RelayCommand]
        private void SaveUser()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) return;

            var user = SelectedUser ?? new User();
            user.Username = Username;
            user.Password = Password;
            user.FullName = FullName;
            user.Role = Role;
            user.IsActive = IsActive;
            user.Notes = Notes;

            App.Database.SaveUser(user);
            LoadUsers();
            ClearFields();
        }

        [RelayCommand]
        private void DeleteUser()
        {
            if (SelectedUser == null) return;
            App.Database.DeleteUser(SelectedUser.Id);
            LoadUsers();
            ClearFields();
        }

        [RelayCommand]
        private void ClearFields()
        {
            SelectedUser = null;
            Username = string.Empty;
            Password = string.Empty;
            FullName = string.Empty;
            Role = "Cashier";
            IsActive = true;
            Notes = string.Empty;
        }

        partial void OnSelectedUserChanged(User? value)
        {
            if (value != null)
            {
                Username = value.Username;
                Password = value.Password;
                FullName = value.FullName;
                Role = value.Role;
                IsActive = value.IsActive;
                Notes = value.Notes;
            }
            else
            {
                ClearFields();
            }
        }
    }
}
