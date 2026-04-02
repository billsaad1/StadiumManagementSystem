using System.Windows;

namespace StadiumManagementSystem.Helpers
{
    public static class LocalizationHelper
    {
        public static void SetLanguage(string languageCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (languageCode)
            {
                case "ar":
                    dict.Source = new Uri("pack://application:,,,/Resources/StringResources.ar.xaml", UriKind.Absolute);
                    break;
                default:
                    dict.Source = new Uri("pack://application:,,,/Resources/StringResources.en.xaml", UriKind.Absolute);
                    break;
            }

            // Remove old string resource dictionaries
            var oldDicts = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null && d.Source.OriginalString.Contains("StringResources"))
                .ToList();
            
            foreach (var oldDict in oldDicts)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
