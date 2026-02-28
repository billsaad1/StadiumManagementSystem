using System.Windows;
using System.Windows.Media;

namespace StadiumManagementSystem.Helpers
{
    public static class ThemeHelper
    {
        public static void ApplyTheme(string colorHex)
        {
            try
            {
                var converter = new BrushConverter();
                var primaryBrush = (Brush)converter.ConvertFromString(colorHex)!;

                Application.Current.Resources["PrimaryColor"] = primaryBrush;

                // Create a slightly darker version for PrimaryColorDark
                if (primaryBrush is SolidColorBrush solidBrush)
                {
                    var color = solidBrush.Color;
                    var darkColor = Color.FromRgb(
                        (byte)(color.R * 0.8),
                        (byte)(color.G * 0.8),
                        (byte)(color.B * 0.8)
                    );
                    Application.Current.Resources["PrimaryColorDark"] = new SolidColorBrush(darkColor);
                }
                else
                {
                    Application.Current.Resources["PrimaryColorDark"] = primaryBrush;
                }
            }
            catch
            {
                // Fallback to default if invalid color
            }
        }
    }
}
