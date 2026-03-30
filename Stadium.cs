namespace StadiumManagementSystem.Models
{
    public class Stadium
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MorningPrice { get; set; } = 8000;
        public decimal EveningPrice { get; set; } = 10000;
        public bool IsActive { get; set; } = true;
    }
}
