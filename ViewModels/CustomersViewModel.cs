using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StadiumManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows;
using System.Linq;

namespace StadiumManagementSystem.ViewModels
{
    public partial class CustomersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Customer> _customers = new();

        public CustomersViewModel()
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var list = App.Database.GetCustomers();
            Customers = new ObservableCollection<Customer>(list);
        }

        [RelayCommand]
        private void PrintStatement(Customer customer)
        {
            if (customer == null) return;

            var bookings = App.Database.GetBookings().Where(b => b.CustomerId == customer.Id).ToList();

            bool isArabic = System.Windows.Application.Current.Resources.MergedDictionaries
                .Any(d => d.Source != null && d.Source.OriginalString.Contains("ar.xaml"));

            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.FontFamily = new System.Windows.Media.FontFamily(isArabic ? "Traditional Arabic" : "Segoe UI");
            doc.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            Paragraph header = new Paragraph(new Run(isArabic ? "كشف حساب عميل" : "Customer Statement"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(header);

            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "العميل" : "Customer")}: {customer.Name}")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "الهاتف" : "Phone")}: {customer.Phone}")));
            doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")));

            Table table = new Table();
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.RowGroups.Add(new TableRowGroup());

            var headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run(isArabic ? "التاريخ" : "Date"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run(isArabic ? "المبلغ" : "Amount"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run(isArabic ? "الحالة" : "Status"))));
            table.RowGroups[0].Rows.Add(headerRow);

            foreach (var b in bookings)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(b.BookingDate.ToShortDateString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(b.TotalPrice.ToString("N0")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(b.PaymentStatus))));
                table.RowGroups[0].Rows.Add(row);
            }
            doc.Blocks.Add(table);

            doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "إجمالي المبلغ" : "Total Spent")}: {customer.TotalSpent:N0} YER")) { FontWeight = FontWeights.Bold });

            var preview = new Views.ReceiptPreviewWindow(doc);
            preview.ShowDialog();
        }
    }
}
