using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using StadiumManagementSystem.Models;
using System.Linq;

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
        private ObservableCollection<Expense> _expenses = new();

        // New Expense properties
        [ObservableProperty]
        private string _newExpenseDescription = string.Empty;
        [ObservableProperty]
        private string _newExpenseCategory = string.Empty;
        [ObservableProperty]
        private decimal _newExpenseAmount;

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

            var expenseList = App.Database.GetExpenses(StartDate, EndDate);
            Expenses = new ObservableCollection<Expense>(expenseList);
            TotalExpenses = expenseList.Sum(e => e.Amount);

            NetProfit = TotalRevenue - TotalExpenses;
        }

        [RelayCommand]
        private void AddExpense()
        {
            if (string.IsNullOrWhiteSpace(NewExpenseDescription) || NewExpenseAmount <= 0) return;

            var expense = new Expense
            {
                Description = NewExpenseDescription,
                Category = NewExpenseCategory,
                Amount = NewExpenseAmount,
                Date = DateTime.Today
            };

            App.Database.SaveExpense(expense);

            // Clear inputs
            NewExpenseDescription = string.Empty;
            NewExpenseCategory = string.Empty;
            NewExpenseAmount = 0;

            LoadData();
        }

        [RelayCommand]
        private void DeleteExpense(Expense expense)
        {
            if (expense == null) return;
            App.Database.DeleteExpense(expense.Id);
            LoadData();
        }

        [RelayCommand]
        private void PrintDailyClosing()
        {
            var settings = App.Database.GetSettings();
            bool isArabic = System.Windows.Application.Current.Resources.MergedDictionaries
                .Any(d => d.Source != null && d.Source.OriginalString.Contains("ar.xaml"));

            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.FontFamily = new System.Windows.Media.FontFamily(isArabic ? "Traditional Arabic" : "Segoe UI");
            doc.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            Paragraph header = new Paragraph(new Run(isArabic ? "تقرير الإغلاق اليومي" : "Daily Closing Report"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(header);

            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "الفترة" : "Period")}: {StartDate:d} - {EndDate:d}")));
            doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")));

            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "إجمالي الإيرادات" : "Total Revenue")}: {TotalRevenue:N0} YER")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "إجمالي المصاريف" : "Total Expenses")}: {TotalExpenses:N0} YER")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "صافي الربح" : "Net Profit")}: {NetProfit:N0} YER")) { FontWeight = FontWeights.Bold });

            var preview = new Views.ReceiptPreviewWindow(doc);
            preview.ShowDialog();
        }
    }
}
