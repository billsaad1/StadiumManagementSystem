using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.Helpers
{
    public static class PrintHelper
    {
        public static FlowDocument CreateReceiptDocument(Booking booking, Settings settings)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.FontFamily = new FontFamily("Traditional Arabic, Segoe UI");
            doc.FlowDirection = FlowDirection.RightToLeft;

            // Header Grid
            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Logo
            if (!string.IsNullOrEmpty(settings.LogoPath) && System.IO.File.Exists(settings.LogoPath))
            {
                try {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(System.IO.Path.GetFullPath(settings.LogoPath)));
                    var image = new Image { Source = bitmap, Width = 100, Height = 100 };
                    Grid.SetColumn(image, 1);
                    headerGrid.Children.Add(image);
                } catch {}
            }

            // Organization Info (Right side in RTL)
            StackPanel rightInfo = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            rightInfo.Children.Add(new TextBlock { Text = settings.OrganizationName, FontSize = 22, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Left });
            rightInfo.Children.Add(new TextBlock { Text = settings.Location, FontSize = 12, HorizontalAlignment = HorizontalAlignment.Left });
            rightInfo.Children.Add(new TextBlock { Text = settings.Phone, FontSize = 12, HorizontalAlignment = HorizontalAlignment.Left });
            Grid.SetColumn(rightInfo, 0);
            headerGrid.Children.Add(rightInfo);

            // Title Section (Left side in RTL)
            StackPanel leftInfo = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right };
            leftInfo.Children.Add(new TextBlock { Text = "سند قبض", FontSize = 26, FontWeight = FontWeights.Bold });
            leftInfo.Children.Add(new TextBlock { Text = "CASH RECEIPT", FontSize = 16, FontWeight = FontWeights.SemiBold });
            Grid.SetColumn(leftInfo, 2);
            headerGrid.Children.Add(leftInfo);

            doc.Blocks.Add(new BlockUIContainer(headerGrid));
            doc.Blocks.Add(new Paragraph(new Run("----------------------------------------------------------------------------------------------------")) { TextAlignment = TextAlignment.Center });

            // Details Table
            Table table = new Table { CellSpacing = 5, Margin = new Thickness(0, 20, 0, 20) };
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });

            TableRowGroup group = new TableRowGroup();

            TableRow row1 = new TableRow();
            row1.Cells.Add(CreateCell("رقم السند / No:", true));
            row1.Cells.Add(CreateCell(booking.BookingNumber));
            row1.Cells.Add(CreateCell("التاريخ / Date:", true));
            row1.Cells.Add(CreateCell(booking.BookingDate.ToString("yyyy-MM-dd")));
            group.Rows.Add(row1);

            TableRow row2 = new TableRow();
            row2.Cells.Add(CreateCell("العميل / Client:", true));
            row2.Cells.Add(CreateCell(booking.CustomerName, false, 3)); // Spans 3 columns
            group.Rows.Add(row2);

            TableRow row3 = new TableRow();
            row3.Cells.Add(CreateCell("الملعب / Stadium:", true));
            row3.Cells.Add(CreateCell(booking.Stadium));
            row3.Cells.Add(CreateCell("الوقت / Time:", true));
            row3.Cells.Add(CreateCell(booking.TimeSlot));
            group.Rows.Add(row3);

            table.RowGroups.Add(group);
            doc.Blocks.Add(table);

            doc.Blocks.Add(new Paragraph(new Run("----------------------------------------------------------------------------------------------------")) { TextAlignment = TextAlignment.Center });

            // Financials
            Table finTable = new Table { CellSpacing = 10 };
            finTable.Columns.Add(new TableColumn { Width = new GridLength(200) });
            finTable.Columns.Add(new TableColumn { Width = new GridLength(200) });
            TableRowGroup finGroup = new TableRowGroup();

            finGroup.Rows.Add(CreateFinancialRow("المبلغ الإجمالي / Total:", $"{booking.TotalPrice:N0} YER", Brushes.Black));
            finGroup.Rows.Add(CreateFinancialRow("المبلغ المدفوع / Paid:", $"{booking.Deposit:N0} YER", Brushes.Green));
            finGroup.Rows.Add(CreateFinancialRow("المتبقي / Balance:", $"{booking.Balance:N0} YER", Brushes.Red));

            finTable.RowGroups.Add(finGroup);
            doc.Blocks.Add(finTable);

            // Footer
            doc.Blocks.Add(new Paragraph(new Run("\n\n")));

            Grid footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            StackPanel signature1 = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            signature1.Children.Add(new TextBlock { Text = "توقيع المحاسب", FontWeight = FontWeights.Bold });
            signature1.Children.Add(new TextBlock { Text = "Accountant Signature", FontSize = 10 });
            Grid.SetColumn(signature1, 0);
            footerGrid.Children.Add(signature1);

            StackPanel signature2 = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
            signature2.Children.Add(new TextBlock { Text = "توقيع المستلم", FontWeight = FontWeights.Bold });
            signature2.Children.Add(new TextBlock { Text = "Recipient Signature", FontSize = 10 });
            Grid.SetColumn(signature2, 1);
            footerGrid.Children.Add(signature2);

            doc.Blocks.Add(new BlockUIContainer(footerGrid));

            doc.Blocks.Add(new Paragraph(new Run("\n----------------------------------------------------------------------------------------------------")) { TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Run($"{settings.Address} | Phone: {settings.Phone} | {settings.Email}")) { TextAlignment = TextAlignment.Center, FontSize = 10 });

            return doc;
        }

        private static TableCell CreateCell(string text, bool isBold = false, int columnSpan = 1)
        {
            var cell = new TableCell(new Paragraph(new Run(text)) { FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal });
            if (columnSpan > 1) cell.ColumnSpan = columnSpan;
            cell.Padding = new Thickness(2);
            return cell;
        }

        private static TableRow CreateFinancialRow(string label, string value, Brush foreground)
        {
            TableRow row = new TableRow();
            row.Cells.Add(new TableCell(new Paragraph(new Run(label)) { FontWeight = FontWeights.Bold, FontSize = 16 }));
            row.Cells.Add(new TableCell(new Paragraph(new Run(value)) { FontWeight = FontWeights.Bold, FontSize = 18, Foreground = foreground }));
            return row;
        }

        public static void PrintReceipt(Booking booking, Settings settings)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = CreateReceiptDocument(booking, settings);
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
