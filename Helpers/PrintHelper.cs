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

                // Header Grid
                Grid headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Logo
                if (!string.IsNullOrEmpty(settings.LogoPath) && System.IO.File.Exists(settings.LogoPath))
                {
                    try {
                        var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(settings.LogoPath));
                        var image = new Image { Source = bitmap, Width = 80, Height = 80 };
                        Grid.SetColumn(image, 1);
                        headerGrid.Children.Add(image);
                    } catch {}
                }

                // Title Section
                StackPanel leftInfo = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                leftInfo.Children.Add(new TextBlock { Text = settings.OrganizationName, FontSize = 20, FontWeight = FontWeights.Bold });
                leftInfo.Children.Add(new TextBlock { Text = settings.Location, FontSize = 10 });
                Grid.SetColumn(leftInfo, 0);
                headerGrid.Children.Add(leftInfo);

                StackPanel rightInfo = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
                rightInfo.Children.Add(new TextBlock { Text = "سند قبض", FontSize = 20, FontWeight = FontWeights.Bold, FlowDirection = FlowDirection.RightToLeft });
                rightInfo.Children.Add(new TextBlock { Text = "CASH RECEIPT", FontSize = 14, FontWeight = FontWeights.SemiBold });
                Grid.SetColumn(rightInfo, 2);
                headerGrid.Children.Add(rightInfo);

                doc.Blocks.Add(new BlockUIContainer(headerGrid));
                doc.Blocks.Add(new Paragraph(new Run("----------------------------------------------------------------------------------------------------")));

                // Details Table
                Table table = new Table { CellSpacing = 10 };
                table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
                table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

                TableRowGroup group = new TableRowGroup();

                group.Rows.Add(CreateRow("Receipt # / رقم السند:", booking.BookingNumber));
                group.Rows.Add(CreateRow("Date / التاريخ:", booking.BookingDate.ToString("yyyy-MM-dd")));
                group.Rows.Add(CreateRow("Customer / العميل:", booking.CustomerName));
                group.Rows.Add(CreateRow("Stadium / الملعب:", booking.Stadium));
                group.Rows.Add(CreateRow("Time / الوقت:", booking.TimeSlot));

                table.RowGroups.Add(group);
                doc.Blocks.Add(table);

                doc.Blocks.Add(new Paragraph(new Run("----------------------------------------------------------------------------------------------------")));

                // Financials
                Section financials = new Section();
                financials.Blocks.Add(new Paragraph(new Run($"Total Price / المبلغ الإجمالي: {booking.TotalPrice:N0} YER")) { FontSize = 14 });
                financials.Blocks.Add(new Paragraph(new Run($"Paid Amount / المبلغ المدفوع: {booking.Deposit:N0} YER")) { FontSize = 14, FontWeight = FontWeights.Bold, Foreground = Brushes.Green });
                financials.Blocks.Add(new Paragraph(new Run($"Remaining / المتبقي: {booking.Balance:N0} YER")) { FontSize = 14, FontWeight = FontWeights.Bold, Foreground = Brushes.Red });
                doc.Blocks.Add(financials);

                // Footer
                doc.Blocks.Add(new Paragraph(new Run("\n\n")));
                doc.Blocks.Add(new Paragraph(new Run("Recipient Signature / توقيع المستلم")) { TextAlignment = TextAlignment.Right, Margin = new Thickness(0,0,50,0) });

                doc.Blocks.Add(new Paragraph(new Run("----------------------------------------------------------------------------------------------------")));
                doc.Blocks.Add(new Paragraph(new Run($"{settings.Address} | Phone: {settings.Phone}")) { TextAlignment = TextAlignment.Center, FontSize = 10 });

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Stadium Receipt");
            }
        }

        private static TableRow CreateRow(string label, string value)
        {
            TableRow row = new TableRow();
            row.Cells.Add(new TableCell(new Paragraph(new Run(label)) { FontWeight = FontWeights.SemiBold }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(value))));
            return row;
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
