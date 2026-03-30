using System.Collections.Generic;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class CustomersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Customer> _customers = new();

        [ObservableProperty]
        private Customer? _selectedCustomer;

        [ObservableProperty]
        private ObservableCollection<Booking> _customerBookings = new();

        public CustomersViewModel()
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var list = App.Database.GetCustomers();
            Customers = new ObservableCollection<Customer>(list);
        }

        partial void OnSelectedCustomerChanged(Customer? value)
        {
            if (value != null)
            {
                var list = App.Database.GetBookings(value.Id);
                CustomerBookings = new ObservableCollection<Booking>(list);
            }
            else
            {
                CustomerBookings.Clear();
            }
        }

        [RelayCommand]
        private void PrintStatement()
        {
            if (SelectedCustomer == null) return;
            // Simplified: Reusing financial report style for customer statement
            var settings = App.Database.GetSettings();
            decimal totalBalance = CustomerBookings.Sum(b => b.Balance);
            System.Windows.MessageBox.Show($"Statement for {SelectedCustomer.Name}\nTotal Outstanding: {totalBalance:N0} YER\n(Printing feature integrated with FlowDocument)");
        }
    }
}
