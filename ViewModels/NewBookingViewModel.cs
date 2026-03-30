using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
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
        private string _stadium = "Stadium 1";

        [ObservableProperty]
        private int _startHour = 10;

        [ObservableProperty]
        private int _endHour = 11;

        [ObservableProperty]
        private decimal _totalPrice;

        [ObservableProperty]
        private decimal _deposit;

        [ObservableProperty]
        private decimal _balance;

        [ObservableProperty]
        private string _paymentMethod = "Cash";

        [ObservableProperty]
        private ObservableCollection<string> _paymentMethods = new() { "Cash", "Bank Transfer", "Mobile Money" };

        [ObservableProperty]
        private ObservableCollection<Stadium> _stadiums = new();

        private Settings _settings;

        public NewBookingViewModel()
        {
            _settings = App.Database.GetSettings();
            Stadiums = new ObservableCollection<Stadium>(App.Database.GetStadiums());
            UpdatePrice();
        }

        partial void OnStartHourChanged(int value) => UpdatePrice();
        partial void OnEndHourChanged(int value) => UpdatePrice();
        partial void OnStadiumChanged(string value) => UpdatePrice();
        partial void OnTotalPriceChanged(decimal value) => UpdateBalance();
        partial void OnDepositChanged(decimal value) => UpdateBalance();

        private void UpdatePrice()
        {
            int duration = EndHour - StartHour + 1;
            if (duration < 1) duration = 0;

            var selected = Stadiums.FirstOrDefault(s => s.Name == Stadium);
            decimal pricePerHour = selected?.HourlyPrice ?? _settings.Stadium1Price;

            TotalPrice = duration * pricePerHour;
        }

        private void UpdateBalance()
        {
            Balance = TotalPrice - Deposit;
        }

        public event Action<bool>? RequestClose;

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(CustomerName)) return;
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

            if (!App.Database.CheckAvailability(Stadium, BookingDate, StartHour, EndHour))
            {
                System.Windows.MessageBox.Show("Time slot is already booked!");
                return;
            }

            var booking = new Booking
            {
                BookingNumber = $"BK-{BookingDate:yyMMdd}-S{(Stadium.Contains("1") ? "1" : "2")}-H{StartHour:D2}",
                BookingDate = BookingDate,
                Stadium = Stadium,
                StartHour = StartHour,
                EndHour = EndHour,
                Duration = EndHour - StartHour + 1,
                TimeSlot = $"{StartHour:D2}:00 - {(EndHour + 1):D2}:00",
                CustomerName = CustomerName,
                CustomerPhone = CustomerPhone,
                TotalPrice = TotalPrice,
                Deposit = Deposit,
                Balance = Balance,
                PaymentMethod = PaymentMethod,
                PaymentStatus = Balance <= 0 ? "Paid" : (Deposit > 0 ? "Partial" : "Pending")
            };

            App.Database.SaveBooking(booking);
            RequestClose?.Invoke(true);
        }
    }
}
