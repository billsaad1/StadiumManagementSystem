namespace StadiumManagementSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = "Regular";
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastBooking { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
