using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using StadiumManagementSystem.Models;

namespace StadiumManagementSystem.ViewModels
{
    public class ScheduleSlot : ObservableObject
    {
        public int Hour { get; set; }
        public string TimeRange { get; set; } = string.Empty;
        
        private string _status = "Available";
        public string Status 
        { 
            get => _status; 
            set => SetProperty(ref _status, value); 
        }

        private string _customer = "";
        public string Customer 
        { 
            get => _customer; 
            set => SetProperty(ref _customer, value); 
        }
    }

    public partial class ScheduleViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private string _selectedStadium = "Stadium 1";

        public ObservableCollection<ScheduleSlot> Slots { get; } = new();

        public ScheduleViewModel()
        {
            for (int i = 1; i <= 24; i++)
            {
                Slots.Add(new ScheduleSlot { Hour = i, TimeRange = $"{i:D2}:00 - {(i + 1):D2}:00" });
            }
            LoadSchedule();
        }

        partial void OnSelectedDateChanged(DateTime value) => LoadSchedule();
        partial void OnSelectedStadiumChanged(string value) => LoadSchedule();

        private void LoadSchedule()
        {
            // Reset slots
            foreach (var slot in Slots)
            {
                slot.Status = "Available";
                slot.Customer = "";
            }

            var bookings = App.Database.GetBookings()
                .Where(b => b.BookingDate.Date == SelectedDate.Date && b.Stadium == SelectedStadium);

            foreach (var b in bookings)
            {
                for (int h = b.StartHour; h <= b.EndHour; h++)
                {
                    var slot = Slots.FirstOrDefault(s => s.Hour == h);
                    if (slot != null)
                    {
                        slot.Status = "BOOKED";
                        slot.Customer = b.CustomerName;
                    }
                }
            }
        }
    }
}
