using System;
using System.Windows;

namespace StadiumManagementSystem.Views
{
    public partial class ReceiptPreviewView : Window
    {
        public ReceiptPreviewView()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (DataContext is ViewModels.ReceiptPreviewViewModel vm)
            {
                vm.RequestClose += () => this.Close();
            }
        }
    }
}
