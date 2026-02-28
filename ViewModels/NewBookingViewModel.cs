using System.Collections.Generic;
using System.Linq;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class NewBookingViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _customerName = string.Empty;

        [ObservableProperty]
        private string _customerPhone = string.Empty;

        [ObservableProperty]
        private DateTime _bookingDate = DateTime.Today;

        [ObservableProperty]
        private Stadium? _selectedStadium;

        [ObservableProperty]
        private ObservableCollection<Stadium> _stadiums;

        [ObservableProperty]
        private int _startHour = 10;

        [ObservableProperty]
        private int _endHour = 11;

        [ObservableProperty]
        private decimal _totalPrice;

        private Settings _settings;

        public NewBookingViewModel()
        {
            _settings = App.Database.GetSettings();
            _stadiums = new ObservableCollection<Stadium>(App.Database.GetStadiums().Where(s => s.IsActive));
            SelectedStadium = Stadiums.FirstOrDefault();
            UpdatePrice();
        }

        partial void OnStartHourChanged(int value) => UpdatePrice();
        partial void OnEndHourChanged(int value) => UpdatePrice();
        partial void OnSelectedStadiumChanged(Stadium? value) => UpdatePrice();

        private void UpdatePrice()
        {
            if (SelectedStadium == null) return;

            decimal total = 0;
            for (int h = StartHour; h <= EndHour; h++)
            {
                if (h >= _settings.EveningCutoffHour)
                    total += SelectedStadium.EveningPrice;
                else
                    total += SelectedStadium.MorningPrice;
            }
            TotalPrice = total;
        }

        public event Action<bool>? RequestClose;

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(CustomerName) || SelectedStadium == null) return;
            if (StartHour < 1 || StartHour > 24 || EndHour < 1 || EndHour > 24)
            {
                System.Windows.MessageBox.Show("Hours must be between 1 and 24");
                return;
            }
            if (EndHour < StartHour)
            {
                System.Windows.MessageBox.Show("End hour must be greater than or equal to start hour");
                return;
            }

            if (!App.Database.CheckAvailability(SelectedStadium.Name, BookingDate, StartHour, EndHour))
            {
                System.Windows.MessageBox.Show("Time slot is already booked!");
                return;
            }

            var booking = new Booking
            {
                BookingNumber = $"BK-{BookingDate:yyMMdd}-{SelectedStadium.Name.Substring(0, Math.Min(3, SelectedStadium.Name.Length)).ToUpper()}-H{StartHour:D2}",
                BookingDate = BookingDate,
                Stadium = SelectedStadium.Name,
                StartHour = StartHour,
                EndHour = EndHour,
                Duration = EndHour - StartHour + 1,
                TimeSlot = $"{StartHour:D2}:00 - {(EndHour + 1):D2}:00",
                CustomerName = CustomerName,
                CustomerPhone = CustomerPhone,
                TotalPrice = TotalPrice,
                Deposit = 0, // Simplified for now
                Balance = TotalPrice,
                PaymentStatus = "Pending"
            };

            App.Database.SaveBooking(booking);
            RequestClose?.Invoke(true);
        }
    }
}
