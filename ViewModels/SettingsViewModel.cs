using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private Settings _settings;

        [ObservableProperty]
        private ObservableCollection<Stadium> _stadiums = new();

        [ObservableProperty]
        private Stadium _selectedStadium = new();

        public SettingsViewModel()
        {
            _settings = App.Database.GetSettings();
            LoadStadiums();
        }

        private void LoadStadiums()
        {
            Stadiums = new ObservableCollection<Stadium>(App.Database.GetStadiums());
        }

        [RelayCommand]
        private void Save()
        {
            App.Database.SaveSettings(Settings);
            System.Windows.MessageBox.Show("Settings saved!");
        }

        [RelayCommand]
        private void SaveStadium()
        {
            if (string.IsNullOrWhiteSpace(SelectedStadium.Name)) return;
            App.Database.SaveStadium(SelectedStadium);
            LoadStadiums();
            SelectedStadium = new Stadium();
        }

        [RelayCommand]
        private void EditStadium(Stadium stadium)
        {
            if (stadium == null) return;
            SelectedStadium = stadium;
        }

        [RelayCommand]
        private void DeleteStadium(Stadium stadium)
        {
            if (stadium == null) return;
            App.Database.DeleteStadium(stadium.Id);
            LoadStadiums();
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
