using System.Collections.Generic;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            if (view.ShowDialog() == true)
            {
                LoadBookings();
            }
        }
    }
}
