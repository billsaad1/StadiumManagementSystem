namespace StadiumManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin, Cashier, Viewer
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Notes { get; set; } = string.Empty;
    }
}
