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

        [RelayCommand]
        private void PrintReceipt(Booking booking)
        {
            var settings = App.Database.GetSettings();
            Helpers.PrintHelper.PrintReceipt(booking, settings);
        }

        [RelayCommand]
        private void SendWhatsApp(Booking booking)
        {
            if (string.IsNullOrEmpty(booking.CustomerPhone)) return;

            string message = $"Hello {booking.CustomerName}, regarding your booking {booking.BookingNumber} on {booking.BookingDate:dd/MM}. Status: {booking.PaymentStatus}.";
            string url = $"https://wa.me/{booking.CustomerPhone.Replace("+", "").Replace(" ", "")}?text={Uri.EscapeDataString(message)}";

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
