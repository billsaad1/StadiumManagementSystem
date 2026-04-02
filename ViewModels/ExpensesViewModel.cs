using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace StadiumManagementSystem.ViewModels
{
    public partial class ExpensesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Expense> _expenses = new();

        [ObservableProperty]
        private string _newCategory = string.Empty;

        [ObservableProperty]
        private decimal _newAmount;

        [ObservableProperty]
        private string _newDescription = string.Empty;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today.AddDays(-30);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        public ExpensesViewModel()
        {
            LoadExpenses();
        }

        [RelayCommand]
        public void LoadExpenses()
        {
            var list = App.Database.GetExpenses(StartDate, EndDate);
            Expenses = new ObservableCollection<Expense>(list);
        }

        [RelayCommand]
        private void AddExpense()
        {
            if (string.IsNullOrEmpty(NewCategory) || NewAmount <= 0) return;

            var expense = new Expense
            {
                Category = NewCategory,
                Amount = NewAmount,
                Description = NewDescription,
                ExpenseDate = DateTime.Now,
                CreatedBy = "Admin" // Simplified
            };

            App.Database.SaveExpense(expense);
            LoadExpenses();

            NewCategory = string.Empty;
            NewAmount = 0;
            NewDescription = string.Empty;
        }

        [RelayCommand]
        private void DeleteExpense(Expense expense)
        {
            if (expense != null)
            {
                App.Database.DeleteExpense(expense.Id);
                LoadExpenses();
            }
        }

        partial void OnStartDateChanged(DateTime value) => LoadExpenses();
        partial void OnEndDateChanged(DateTime value) => LoadExpenses();
    }
}
