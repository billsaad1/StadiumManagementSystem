using System.Collections.Generic;
using System.Linq;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class FinancialsViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _startDate = DateTime.Today.AddDays(-30);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private decimal _totalRevenue;

        [ObservableProperty]
        private int _totalBookings;

        public FinancialsViewModel()
        {
            LoadData();
        }

        partial void OnStartDateChanged(DateTime value) => LoadData();
        partial void OnEndDateChanged(DateTime value) => LoadData();

        private void LoadData()
        {
            var bookings = App.Database.GetBookings()
                .Where(b => b.BookingDate.Date >= StartDate.Date && b.BookingDate.Date <= EndDate.Date)
                .ToList();

            TotalBookings = bookings.Count;
            TotalRevenue = bookings.Sum(b => b.TotalPrice);
        }
    }
}
