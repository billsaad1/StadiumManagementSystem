using System.Collections.Generic;
using System.Linq;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private decimal _totalExpenses;

        [ObservableProperty]
        private decimal _netProfit;

        [ObservableProperty]
        private int _totalBookings;

        [ObservableProperty]
        private ObservableCollection<StadiumRevenue> _stadiumRevenues = new();

        [RelayCommand]
        private void PrintReport()
        {
            var settings = App.Database.GetSettings();
            var doc = Helpers.PrintHelper.CreateFinancialReportDocument(StartDate, EndDate, TotalBookings, TotalRevenue, TotalExpenses, NetProfit, settings);
            var vm = new ReceiptPreviewViewModel(doc, "Financial Report Preview");
            var view = new Views.ReceiptPreviewView { DataContext = vm };
            view.ShowDialog();
        }

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

            var expenses = App.Database.GetExpenses(StartDate, EndDate);

            TotalBookings = bookings.Count;
            TotalRevenue = bookings.Sum(b => b.TotalPrice);
            TotalExpenses = expenses.Sum(e => e.Amount);
            NetProfit = TotalRevenue - TotalExpenses;

            var stadiumGroup = bookings.GroupBy(b => b.Stadium)
                .Select(g => new StadiumRevenue { StadiumName = g.Key, Revenue = g.Sum(b => b.TotalPrice) })
                .ToList();
            StadiumRevenues = new ObservableCollection<StadiumRevenue>(stadiumGroup);
        }
    }

    public class StadiumRevenue
    {
        public string StadiumName { get; set; } = "";
        public decimal Revenue { get; set; }
    }
}
