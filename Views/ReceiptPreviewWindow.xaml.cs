using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.Views
{
    public partial class ReceiptPreviewWindow : Window
    {
        private readonly FlowDocument _document;

        public ReceiptPreviewWindow(FlowDocument document)
        {
            InitializeComponent();
            _document = document;
            DocReader.Document = _document;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)_document).DocumentPaginator, "Stadium Receipt");
                DialogResult = true;
                Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
