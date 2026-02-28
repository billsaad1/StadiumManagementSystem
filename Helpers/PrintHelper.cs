using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.Helpers
{
    public static class PrintHelper
    {
        public static void PrintReceipt(Booking booking, Settings settings)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = new FlowDocument();
                doc.PagePadding = new Thickness(50);
                doc.FontFamily = new FontFamily("Segoe UI");

                if (!string.IsNullOrEmpty(settings.LogoPath) && System.IO.File.Exists(settings.LogoPath))
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(settings.LogoPath));
                    var image = new Image { Source = bitmap, Width = 100, Height = 100, HorizontalAlignment = HorizontalAlignment.Center };
                    doc.Blocks.Add(new BlockUIContainer(image));
                }

                Paragraph header = new Paragraph(new Run(settings.OrganizationName))
                {
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                doc.Blocks.Add(header);

                Paragraph subHeader = new Paragraph(new Run($"{settings.Address}, {settings.Location}\nPhone: {settings.Phone} | Manager: {settings.ManagerName}"))
                {
                    FontSize = 12,
                    FontStyle = FontStyles.Italic,
                    TextAlignment = TextAlignment.Center
                };
                doc.Blocks.Add(subHeader);

                doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")) { TextAlignment = TextAlignment.Center });

                Paragraph title = new Paragraph(new Run("RESERVATION RECEIPT"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 20)
                };
                doc.Blocks.Add(title);

                doc.Blocks.Add(new Paragraph(new Run($"Receipt #: {booking.BookingNumber}")));
                doc.Blocks.Add(new Paragraph(new Run($"Date: {booking.BookingDate:d}")));
                doc.Blocks.Add(new Paragraph(new Run($"Stadium: {booking.Stadium}")));
                doc.Blocks.Add(new Paragraph(new Run($"Time: {booking.TimeSlot}")));
                doc.Blocks.Add(new Paragraph(new Run($"Customer: {booking.CustomerName}")));
                doc.Blocks.Add(new Paragraph(new Run($"Phone: {booking.CustomerPhone}")));

                doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")));

                doc.Blocks.Add(new Paragraph(new Run($"Total Price: {booking.TotalPrice} YER")) { FontWeight = FontWeights.Bold });
                doc.Blocks.Add(new Paragraph(new Run($"Paid: {booking.Deposit} YER")));
                doc.Blocks.Add(new Paragraph(new Run($"Balance: {booking.Balance} YER")) { Foreground = Brushes.Red });

                doc.Blocks.Add(new Paragraph(new Run("\nThank you for choosing us!")) { TextAlignment = TextAlignment.Center });

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Stadium Receipt");
            }
        }

        public static void PrintSchedule(DateTime date, string stadium, IEnumerable<ViewModels.ScheduleSlot> slots, Settings settings)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = new FlowDocument();
                doc.PagePadding = new Thickness(50);
                doc.FontFamily = new FontFamily("Segoe UI");

                Paragraph header = new Paragraph(new Run($"{settings.OrganizationName} - SCHEDULE"))
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                doc.Blocks.Add(header);

                doc.Blocks.Add(new Paragraph(new Run($"Date: {date:d} | Stadium: {stadium}")) { TextAlignment = TextAlignment.Center });

                Table table = new Table { CellSpacing = 0, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
                table.Columns.Add(new TableColumn { Width = new GridLength(150) });
                table.Columns.Add(new TableColumn { Width = new GridLength(100) });
                table.Columns.Add(new TableColumn { Width = new GridLength(250) });

                TableRowGroup group = new TableRowGroup();
                TableRow headerRow = new TableRow { FontWeight = FontWeights.Bold, Background = Brushes.LightGray };
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Time Slot"))));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Status"))));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Customer"))));
                group.Rows.Add(headerRow);

                foreach (var slot in slots)
                {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(slot.TimeRange))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(slot.Status))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(slot.Customer))));
                    group.Rows.Add(row);
                }

                table.RowGroups.Add(group);
                doc.Blocks.Add(table);

                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Stadium Schedule");
            }
        }

        public static void PrintFinancialReport(DateTime start, DateTime end, int totalBookings, decimal totalRevenue, Settings settings)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = new FlowDocument();
                doc.PagePadding = new Thickness(50);
                doc.FontFamily = new FontFamily("Segoe UI");

                Paragraph header = new Paragraph(new Run($"{settings.OrganizationName} - FINANCIAL REPORT"))
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                doc.Blocks.Add(header);

                doc.Blocks.Add(new Paragraph(new Run($"Period: {start:d} to {end:d}")) { TextAlignment = TextAlignment.Center });
                doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")) { TextAlignment = TextAlignment.Center });

                Section summary = new Section();
                summary.Blocks.Add(new Paragraph(new Run($"Total Bookings: {totalBookings}")) { FontSize = 16 });
                summary.Blocks.Add(new Paragraph(new Run($"Total Revenue: {totalRevenue:N0} YER")) { FontSize = 16, FontWeight = FontWeights.Bold });
                doc.Blocks.Add(summary);

                doc.Blocks.Add(new Paragraph(new Run("\nReport Generated on: " + DateTime.Now.ToString("g"))));

                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Financial Report");
            }
        }
    }
}
