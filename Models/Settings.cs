namespace StadiumManagementSystem.Models
{
    public class Settings
    {
        public string OrganizationName { get; set; } = "Jeel Al Bena Association";
        public string ManagerName { get; set; } = "Bilal Al Salami";
        public string Location { get; set; } = "Sana'a, Yemen";
        public string Phone { get; set; } = "+967 777 123 456";
        public string Email { get; set; } = "info@jeelalbena.org";
        public string Address { get; set; } = "Main Street, Sana'a";
        public int EveningCutoffHour { get; set; } = 18; // 6 PM
        public string ThemeColor { get; set; } = "#1F4E78"; // Default Blue
        public decimal MinDeposit { get; set; } = 3000;
        public string LogoPath { get; set; } = string.Empty;

        // Legacy properties for backward compatibility during migration
        public decimal Stadium1Price { get; set; } = 8000;
        public decimal Stadium2Price { get; set; } = 8000;
    }
}
