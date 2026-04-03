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
            var settings = App.Database.GetSettings();
            var doc = Helpers.PrintHelper.CreateCustomerStatementDocument(SelectedCustomer, CustomerBookings, settings);
            var vm = new ReceiptPreviewViewModel(doc, "Customer Statement Preview");
            var view = new Views.ReceiptPreviewView { DataContext = vm };
            view.ShowDialog();
        }
    }
}
