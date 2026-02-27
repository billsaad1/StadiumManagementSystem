using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Settings _settings;

        [ObservableProperty]
        private ObservableCollection<Stadium> _stadiums;

        [ObservableProperty]
        private Stadium? _selectedStadium;

        public List<string> ThemeColors { get; } = new() { "#1F4E78", "#28A745", "#DC3545", "#E91E63", "#9C27B0", "#673AB7" };

        public SettingsViewModel()
        {
            _settings = App.Database.GetSettings();
            _stadiums = new ObservableCollection<Stadium>(App.Database.GetStadiums());
        }

        [RelayCommand]
        private void Save()
        {
            App.Database.SaveSettings(Settings);
            StadiumManagementSystem.Helpers.ThemeHelper.ApplyTheme(Settings.ThemeColor);
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

        [RelayCommand]
        private void AddStadium()
        {
            var newStadium = new Stadium { Name = "New Stadium" };
            App.Database.SaveStadium(newStadium);
            Stadiums = new ObservableCollection<Stadium>(App.Database.GetStadiums());
        }

        [RelayCommand]
        private void SaveStadiums()
        {
            foreach (var s in Stadiums)
            {
                App.Database.SaveStadium(s);
            }
            System.Windows.MessageBox.Show("Stadiums saved!");
        }

        [RelayCommand]
        private void DeleteStadium()
        {
            if (SelectedStadium != null)
            {
                App.Database.DeleteStadium(SelectedStadium.Id);
                Stadiums = new ObservableCollection<Stadium>(App.Database.GetStadiums());
            }
        }
    }
}
