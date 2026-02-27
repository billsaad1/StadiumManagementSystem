using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _totalBookingsToday = "0";

        [ObservableProperty]
        private string _totalRevenueToday = "0 YER";

        [ObservableProperty]
        private string _availableSlotsToday = "48";

        [ObservableProperty]
        private ObservableCollection<Booking> _upcomingReminders = new();

        [ObservableProperty]
        private ISeries[] _revenueSeries = Array.Empty<ISeries>();

        [ObservableProperty]
        private Axis[] _xAxes = Array.Empty<Axis>();

        public DashboardViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            var allBookings = App.Database.GetBookings();
            var today = DateTime.Today;
            
            var todayBookings = allBookings.Where(b => b.BookingDate.Date == today).ToList();
            TotalBookingsToday = todayBookings.Count.ToString();
            TotalRevenueToday = todayBookings.Sum(b => b.TotalPrice).ToString("N0") + " YER";
            AvailableSlotsToday = (48 - todayBookings.Sum(b => b.Duration)).ToString();

            // Reminders for tomorrow
            var tomorrow = today.AddDays(1);
            var tomorrowBookings = allBookings.Where(b => b.BookingDate.Date == tomorrow).ToList();
            UpcomingReminders = new ObservableCollection<Booking>(tomorrowBookings);

            // Chart data: Last 7 days
            var last7Days = Enumerable.Range(-6, 7).Select(i => today.AddDays(i)).ToList();
            var revenueData = last7Days.Select(day => allBookings.Where(b => b.BookingDate.Date == day.Date).Sum(b => b.TotalPrice)).ToArray();

            RevenueSeries = new ISeries[]
            {
                new ColumnSeries<decimal>
                {
                    Values = revenueData,
                    Name = "Revenue"
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = last7Days.Select(d => d.ToString("ddd")).ToArray()
                }
            };
        }

        [RelayCommand]
        private void SendReminder(Booking booking)
        {
            if (string.IsNullOrEmpty(booking.CustomerPhone)) return;
            
            string message = $"Hello {booking.CustomerName}, this is a reminder of your reservation at Stadium on {booking.BookingDate:dd/MM} at {booking.TimeSlot}. Thank you!";
            string url = $"https://wa.me/{booking.CustomerPhone}?text={Uri.EscapeDataString(message)}";
            
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
