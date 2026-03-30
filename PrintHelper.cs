using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.Helpers
{
    public static class PrintHelper
    {
        public static FlowDocument CreateReceiptDocument(Booking booking, Settings settings, bool isArabic)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.FontFamily = new FontFamily(isArabic ? "Traditional Arabic" : "Segoe UI");
            doc.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            if (!string.IsNullOrEmpty(settings.LogoPath) && System.IO.File.Exists(settings.LogoPath))
            {
                try
                {
                    var bitmap = new BitmapImage(new Uri(settings.LogoPath));
                    var image = new Image { Source = bitmap, Width = 100, Height = 100, HorizontalAlignment = HorizontalAlignment.Center };
                    doc.Blocks.Add(new BlockUIContainer(image));
                } catch { }
            }

            Paragraph header = new Paragraph(new Run(settings.OrganizationName))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(header);

            Paragraph subHeader = new Paragraph(new Run($"{settings.Location} | { (isArabic ? "المدير" : "Manager") }: {settings.ManagerName}"))
            {
                FontSize = 12,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(subHeader);

            doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")) { TextAlignment = TextAlignment.Center });

            Paragraph title = new Paragraph(new Run(isArabic ? "سند استلام حجز" : "RESERVATION RECEIPT"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 20)
            };
            doc.Blocks.Add(title);

            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "رقم الفاتورة" : "Receipt #")}: {booking.BookingNumber}")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "التاريخ" : "Date")}: {booking.BookingDate:d}")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "الملعب" : "Stadium")}: {booking.Stadium}")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "الوقت" : "Time")}: {booking.TimeSlot}")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "العميل" : "Customer")}: {booking.CustomerName}")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "رقم الهاتف" : "Phone")}: {booking.CustomerPhone}")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "طريقة الدفع" : "Payment Method")}: {booking.PaymentMethod}")));

            doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")));

            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "الإجمالي" : "Total Price")}: {booking.TotalPrice:N0} YER")) { FontWeight = FontWeights.Bold });
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "المدفوع" : "Paid")}: {booking.Deposit:N0} YER")));
            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "المتبقي" : "Balance")}: {booking.Balance:N0} YER")) { Foreground = Brushes.Red, FontWeight = FontWeights.Bold });

            doc.Blocks.Add(new Paragraph(new Run(isArabic ? "\nشكراً لاختياركم لنا!" : "\nThank you for choosing us!")) { TextAlignment = TextAlignment.Center });

            return doc;
        }

        public static void ShowReceiptPreview(Booking booking, Settings settings, bool isArabic)
        {
            var doc = CreateReceiptDocument(booking, settings, isArabic);
            var preview = new Views.ReceiptPreviewWindow(doc);
            preview.ShowDialog();
        }

        public static void PrintSchedule(DateTime date, string stadium, IEnumerable<ViewModels.ScheduleSlot> slots, bool isArabic)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.FontFamily = new FontFamily(isArabic ? "Traditional Arabic" : "Segoe UI");
            doc.FlowDirection = isArabic ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            Paragraph header = new Paragraph(new Run(isArabic ? $"جدول حجز - {stadium}" : $"Booking Schedule - {stadium}"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(header);

            doc.Blocks.Add(new Paragraph(new Run($"{(isArabic ? "التاريخ" : "Date")}: {date:d}")) { TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Run("--------------------------------------------------")) { TextAlignment = TextAlignment.Center });

            Table table = new Table { CellSpacing = 0, BorderThickness = new Thickness(1), BorderBrush = Brushes.Black };
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.RowGroups.Add(new TableRowGroup());

            var headerRow = new TableRow { Background = Brushes.LightGray, FontWeight = FontWeights.Bold };
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run(isArabic ? "الوقت" : "Time"))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black, Padding = new Thickness(5) });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run(isArabic ? "الحالة" : "Status"))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black, Padding = new Thickness(5) });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run(isArabic ? "العميل" : "Customer"))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black, Padding = new Thickness(5) });
            table.RowGroups[0].Rows.Add(headerRow);

            foreach (var slot in slots)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(slot.TimeRange))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black, Padding = new Thickness(5) });
                row.Cells.Add(new TableCell(new Paragraph(new Run(slot.Status))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black, Padding = new Thickness(5) });
                row.Cells.Add(new TableCell(new Paragraph(new Run(slot.Customer))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black, Padding = new Thickness(5) });
                table.RowGroups[0].Rows.Add(row);
            }
            doc.Blocks.Add(table);

            var preview = new Views.ReceiptPreviewWindow(doc);
            preview.ShowDialog();
        }
    }
}
