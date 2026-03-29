using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using System;

namespace StadiumManagementSystem.ViewModels
{
    public partial class EditBookingViewModel : ObservableObject
    {
        [ObservableProperty]
        private Booking _booking;

        [ObservableProperty]
        private decimal _newPayment;

        public event Action<bool>? RequestClose;

        public EditBookingViewModel(Booking booking)
        {
            _booking = booking;
        }

        [RelayCommand]
        private void SavePayment()
        {
            if (NewPayment <= 0) return;

            Booking.Deposit += NewPayment;
            Booking.Balance = Booking.TotalPrice - Booking.Deposit;
            Booking.PaymentStatus = Booking.Balance <= 0 ? "Paid" : "Partial";

            App.Database.UpdateBookingPayment(Booking.Id, Booking.Deposit, Booking.Balance, Booking.PaymentStatus);
            RequestClose?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke(false);
    }
}
