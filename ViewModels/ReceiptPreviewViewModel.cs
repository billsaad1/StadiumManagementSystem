using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using System.Windows.Documents;
using System.Windows.Controls;

namespace StadiumManagementSystem.ViewModels
{
    public partial class ReceiptPreviewViewModel : ObservableObject
    {
        [ObservableProperty]
        private FlowDocument _receiptDocument;

        private Booking _booking;
        private Settings _settings;

        public event Action? RequestClose;

        public ReceiptPreviewViewModel(Booking booking, Settings settings)
        {
            _booking = booking;
            _settings = settings;
            _receiptDocument = Helpers.PrintHelper.CreateReceiptDocument(booking, settings);
        }

        [RelayCommand]
        private void Print()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)ReceiptDocument).DocumentPaginator, "Stadium Receipt");
                    RequestClose?.Invoke();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    "Error occurred during printing: " + ex.Message,
                    "Print Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Close() => RequestClose?.Invoke();
    }
}
