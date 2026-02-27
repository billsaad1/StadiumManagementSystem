using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StadiumManagementSystem.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Settings _settings;

        public SettingsViewModel()
        {
            _settings = App.Database.GetSettings();
        }

        [RelayCommand]
        private void Save()
        {
            App.Database.SaveSettings(Settings);
            System.Windows.MessageBox.Show("Settings saved!");
        }

        [RelayCommand]
        private void UploadLogo()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (dialog.ShowDialog() == true)
            {
                Settings.LogoPath = dialog.FileName;
                OnPropertyChanged(nameof(Settings));
            }
        }
    }
}
