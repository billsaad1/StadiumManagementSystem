using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using System;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class EditBookingViewModel : ObservableObject
    {
        [ObservableProperty]
        private Booking _booking;

        [ObservableProperty]
        private decimal _newPayment;

        [ObservableProperty]
        private ObservableCollection<Payment> _paymentHistory;

        public event Action<bool>? RequestClose;

        public EditBookingViewModel(Booking booking)
        {
            _booking = booking;
            _paymentHistory = new ObservableCollection<Payment>(App.Database.GetPaymentsByBookingId(booking.Id));
        }

        [RelayCommand]
        private void SavePayment()
        {
            if (NewPayment <= 0) return;

            var payment = new Payment
            {
                BookingId = Booking.Id,
                Amount = NewPayment,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Cash",
                Notes = "Manual Payment Update"
            };

            App.Database.AddPayment(payment);

            // Refresh local booking data
            var updatedBooking = App.Database.GetBookingById(Booking.Id);
            if (updatedBooking != null)
            {
                Booking = updatedBooking;
                PaymentHistory = new ObservableCollection<Payment>(App.Database.GetPaymentsByBookingId(Booking.Id));
                NewPayment = 0;
            }
        }

        [RelayCommand]
        private void PrintReceipt()
        {
            var settings = App.Database.GetSettings();
            var doc = Helpers.PrintHelper.CreateReceiptDocument(Booking, settings);
            var vm = new ReceiptPreviewViewModel(doc, "Receipt Preview");
            var view = new Views.ReceiptPreviewView { DataContext = vm };
            view.ShowDialog();
        }

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke(false);
    }
}
