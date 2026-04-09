namespace StadiumManagementSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty; // e.g., BK-260215-S1-H10
        public DateTime BookingDate { get; set; }
        public string Stadium { get; set; } = string.Empty; // Stadium 1, Stadium 2
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public int Duration { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string Status { get; set; } = "Reserved";
        public decimal TotalPrice { get; set; }
        public decimal Deposit { get; set; }
        public decimal Balance { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string PaymentStatus { get; set; } = "Pending"; // Paid, Partial, Pending
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
