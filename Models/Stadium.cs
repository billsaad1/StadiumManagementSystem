namespace StadiumManagementSystem.Models
{
    public class Stadium
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal HourlyPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public string Notes { get; set; } = string.Empty;
    }
}
