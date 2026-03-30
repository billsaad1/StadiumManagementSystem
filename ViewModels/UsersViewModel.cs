using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<User> _users = new();

        [ObservableProperty]
        private User _selectedUser = new();

        [ObservableProperty]
        private string _passwordInput = string.Empty;

        public UsersViewModel()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            var list = App.Database.GetUsers();
            Users = new ObservableCollection<User>(list);
        }

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(SelectedUser.Username)) return;

            App.Database.SaveUser(SelectedUser, string.IsNullOrWhiteSpace(PasswordInput) ? null : PasswordInput);
            PasswordInput = string.Empty;
            LoadUsers();
            SelectedUser = new User();
            System.Windows.MessageBox.Show("User saved!");
        }

        [RelayCommand]
        private void Delete(User user)
        {
            if (user == null) return;
            if (user.Username == "admin")
            {
                System.Windows.MessageBox.Show("Cannot delete admin user");
                return;
            }
            App.Database.DeleteUser(user.Id);
            LoadUsers();
        }

        [RelayCommand]
        private void Edit(User user)
        {
            if (user == null) return;
            SelectedUser = user;
        }

        [RelayCommand]
        private void Clear()
        {
            SelectedUser = new User();
            PasswordInput = string.Empty;
        }
    }
}
