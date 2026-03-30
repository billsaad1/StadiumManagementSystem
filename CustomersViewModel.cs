using System.Collections.Generic;
using StadiumManagementSystem.Data;
using StadiumManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace StadiumManagementSystem.ViewModels
{
    public partial class CustomersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Customer> _customers = new();

        public CustomersViewModel()
        {
            var list = App.Database.GetCustomers();
            Customers = new ObservableCollection<Customer>(list);
        }
    }
}
