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
        private decimal _newDeposit;

        [ObservableProperty]
        private string _newStatus;

        public event Action<bool>? RequestClose;

        public EditBookingViewModel(Booking booking)
        {
            Booking = booking;
            NewDeposit = booking.Deposit;
            NewStatus = booking.PaymentStatus;
        }

        [RelayCommand]
        private void Save()
        {
            decimal balance = Booking.TotalPrice - NewDeposit;
            string status = balance <= 0 ? "Paid" : (NewDeposit > 0 ? "Partial" : "Pending");

            App.Database.UpdateBookingPayment(Booking.Id, NewDeposit, balance, status);
            RequestClose?.Invoke(true);
        }
    }
}
