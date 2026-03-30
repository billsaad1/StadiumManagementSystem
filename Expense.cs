using System;

namespace StadiumManagementSystem.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., Maintenance, Salary, Utilities
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public string Notes { get; set; } = string.Empty;
    }
}
