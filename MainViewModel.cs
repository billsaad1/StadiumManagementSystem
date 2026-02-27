using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StadiumManagementSystem.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject? _currentViewModel;

        public MainViewModel()
        {
            NavigateToDashboard();
        }

        [RelayCommand]
        private void NavigateToDashboard() => CurrentViewModel = new DashboardViewModel();

        [RelayCommand]
        private void NavigateToBookings() => CurrentViewModel = new BookingsViewModel();

        [RelayCommand]
        private void NavigateToCustomers() => CurrentViewModel = new CustomersViewModel();

        [RelayCommand]
        private void NavigateToSettings() => CurrentViewModel = new SettingsViewModel();

        [RelayCommand]
        private void NavigateToSchedule() => CurrentViewModel = new ScheduleViewModel();

        [RelayCommand]
        private void NavigateToFinancials() => CurrentViewModel = new FinancialsViewModel();

        [RelayCommand]
        private void ToggleLanguage()
        {
            string current = System.Windows.Application.Current.Resources.MergedDictionaries
                .Any(d => d.Source != null && d.Source.OriginalString.Contains(".ar.xaml")) ? "ar" : "en";
            
            Helpers.LocalizationHelper.SetLanguage(current == "en" ? "ar" : "en");
        }
    }
}
