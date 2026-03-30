using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using StadiumManagementSystem.Helpers;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class BookingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Booking> _recentBookings = new();

        public BookingsViewModel()
        {
            LoadBookings();
        }

        private void LoadBookings()
        {
            var list = App.Database.GetBookings();
            RecentBookings = new ObservableCollection<Booking>(list);
        }

        [RelayCommand]
        private void NewBooking()
        {
            var vm = new NewBookingViewModel();
            var view = new Views.NewBookingView { DataContext = vm };
            vm.RequestClose += (success) => { if(success) view.DialogResult = true; view.Close(); };
            if (view.ShowDialog() == true)
            {
                LoadBookings();
            }
        }

        [RelayCommand]
        private void PrintReceipt(Booking booking)
        {
            if (booking == null) return;
            var settings = App.Database.GetSettings();
            bool isArabic = System.Windows.Application.Current.Resources.MergedDictionaries
                .Any(d => d.Source != null && d.Source.OriginalString.Contains("ar.xaml"));
            PrintHelper.ShowReceiptPreview(booking, settings, isArabic);
        }

        [RelayCommand]
        private void EditBooking(Booking booking)
        {
            if (booking == null) return;
            var vm = new EditBookingViewModel(booking);
            var view = new Views.EditBookingView { DataContext = vm };
            vm.RequestClose += (success) => { if(success) view.DialogResult = true; view.Close(); };
            if (view.ShowDialog() == true)
            {
                LoadBookings();
            }
        }
    }
}
