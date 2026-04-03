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
            var doc = Helpers.PrintHelper.CreateReceiptDocument(booking, settings);
            var vm = new ReceiptPreviewViewModel(doc, "Receipt Preview");
            var view = new Views.ReceiptPreviewView { DataContext = vm };
            view.ShowDialog();
        }

        [RelayCommand]
        private void EditBooking(Booking booking)
        {
            var fullBooking = App.Database.GetBookingById(booking.Id);
            if (fullBooking == null) return;

            var vm = new EditBookingViewModel(fullBooking);
            var view = new Views.EditBookingView { DataContext = vm };
            if (view.ShowDialog() == true)
            {
                LoadBookings();
            }
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
