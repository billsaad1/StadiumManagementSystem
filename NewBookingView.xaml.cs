using System.Windows;

namespace StadiumManagementSystem.Views
{
    public partial class NewBookingView : Window
    {
        public NewBookingView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is ViewModels.NewBookingViewModel vm)
                {
                    vm.RequestClose += (result) =>
                    {
                        DialogResult = result;
                        Close();
                    };
                }
            };
        }
    }
}
