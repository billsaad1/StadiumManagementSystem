using System.Windows;

namespace StadiumManagementSystem.Views
{
    public partial class EditBookingView : Window
    {
        public EditBookingView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is ViewModels.EditBookingViewModel vm)
                {
                    vm.RequestClose += (result) =>
                    {
                        if (result) DialogResult = true;
                        Close();
                    };
                }
            };
        }
    }
}
