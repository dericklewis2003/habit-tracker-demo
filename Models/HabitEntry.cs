namespace HabbitTrackerRazor.Models
{
    public class HabitEntry
    {
        public int Id { get; set; }
        public int Day { get; set; }
        public string HabitName { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
} 