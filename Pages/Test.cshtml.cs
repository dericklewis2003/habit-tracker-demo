using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using HabbitTrackerRazor.Models;
using Microsoft.AspNetCore.Http;

namespace HabbitTrackerRazor.Pages
{
    public class TestModel : PageModel
    {
        private readonly ILogger<TestModel> _logger;
        private readonly HabbitTrackerContext _db;

        public TestModel(ILogger<TestModel> logger, HabbitTrackerContext db)
        {
            _logger = logger;
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int Month { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; }
        public int DaysInMonth { get; set; }
        public List<int> Days { get; set; }
        public List<string> Habits { get; set; }
        public List<MemorableMoment> MemorableMoments { get; set; }
        public List<HabitEntry> HabitEntries { get; set; }
        public List<int> HabitsCompletedPerDay { get; set; }
        public string Username { get; set; }
        public int CurrentUserId { get; set; }

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrWhiteSpace(Username))
            {
                // Not logged in
                return;
            }

            // Get user
            var user = _db.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null) return;

            CurrentUserId = user.Id;

            // Default to 2025
            if (Year == 0) Year = 2025;
            if (Month == 0) Month = DateTime.Now.Month;
            
            DaysInMonth = DateTime.DaysInMonth(Year, Month);
            Days = Enumerable.Range(1, DaysInMonth).ToList();

            // Load habits for 2025
            Habits = _db.HabitEntries
                .Where(e => e.UserId == CurrentUserId && e.Year == 2025 && !string.IsNullOrEmpty(e.HabitName))
                .Select(e => e.HabitName)
                .Distinct()
                .ToList();

            // Load memorable moments for current month in 2025
            MemorableMoments = _db.MemorableMoments
                .Where(m => m.UserId == CurrentUserId && m.Month == Month && m.Year == 2025)
                .ToList();

            // Load habit entries for current month in 2025
            HabitEntries = _db.HabitEntries
                .Where(e => e.UserId == CurrentUserId && e.Month == Month && e.Year == 2025)
                .ToList();

            // Calculate completed habits per day for graph
            HabitsCompletedPerDay = new List<int>();
            foreach (var day in Days)
            {
                int completed = HabitEntries.Count(e => e.Day == day && e.IsCompleted);
                HabitsCompletedPerDay.Add(completed);
            }
        }
    }
} 