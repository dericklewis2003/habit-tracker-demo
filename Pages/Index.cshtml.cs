using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using HabbitTrackerRazor.Models;
using Microsoft.AspNetCore.Http;

namespace HabbitTrackerRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HabbitTrackerContext _db;

        public IndexModel(ILogger<IndexModel> logger, HabbitTrackerContext db)
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
        [BindProperty]
        public string NewHabit { get; set; }
        public List<int> HabitsCompletedPerDay { get; set; }
        [BindProperty]
        public string Username { get; set; }
        public int CurrentUserId { get; set; }
        [BindProperty(SupportsGet = false)]
        public Dictionary<string, string> Notes { get; set; }
        [BindProperty(SupportsGet = false)]
        public Dictionary<string, Dictionary<string, string>> HabitChecks { get; set; }

        public void OnGet(int? monthParam, int? yearParam)
        {
            Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrWhiteSpace(Username))
            {
                // Not logged in, show login form only
                Year = yearParam ?? 2025; // Default to 2025
                Month = monthParam ?? DateTime.Now.Month;
                DaysInMonth = DateTime.DaysInMonth(Year, Month);
                Days = Enumerable.Range(1, DaysInMonth).ToList();
                Habits = new List<string>();
                MemorableMoments = new List<MemorableMoment>();
                HabitEntries = new List<HabitEntry>();
                HabitsCompletedPerDay = new List<int>();
                return;
            }

            // Get or create user
            var user = _db.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null)
            {
                user = new User { Username = Username };
                _db.Users.Add(user);
                _db.SaveChanges();
            }
            CurrentUserId = user.Id;

            // Use route parameters if provided
            Year = yearParam ?? 2025;
            Month = monthParam ?? DateTime.Now.Month;
            // Calculate correct days for the specific month
            DaysInMonth = DateTime.DaysInMonth(Year, Month);
            Days = Enumerable.Range(1, DaysInMonth).ToList();

            // Load existing habits for this user in 2025
            Habits = _db.HabitEntries
                .Where(e => e.UserId == CurrentUserId && e.Year == 2025 && !string.IsNullOrEmpty(e.HabitName))
                .Select(e => e.HabitName)
                .Distinct()
                .ToList();

            // If no habits exist, create default ones
            if (Habits.Count == 0)
            {
                var defaultHabits = new List<string> { "Exercise", "Read", "Meditate" };
                foreach (var habit in defaultHabits)
                {
                    // Create entries for all days in 2025 for this habit
                    for (int month = 1; month <= 12; month++)
                    {
                        int daysInThisMonth = DateTime.DaysInMonth(2025, month);
                        for (int day = 1; day <= daysInThisMonth; day++)
                        {
                            var entry = new HabitEntry 
                            { 
                                Day = day, 
                                HabitName = habit, 
                                IsCompleted = false, 
                                UserId = CurrentUserId, 
                                Month = month, 
                                Year = 2025 
                            };
                            _db.HabitEntries.Add(entry);
                        }
                    }
                }
                _db.SaveChanges();
                Habits = defaultHabits;
            }

            // Load memorable moments for current month
            MemorableMoments = _db.MemorableMoments
                .Where(m => m.UserId == CurrentUserId && m.Month == Month && m.Year == 2025)
                .ToList();

            // Ensure we have a moment entry for each day in current month
            for (int d = 1; d <= DaysInMonth; d++)
            {
                if (!MemorableMoments.Any(m => m.Day == d))
                {
                    MemorableMoments.Add(new MemorableMoment 
                    { 
                        Day = d, 
                        Note = string.Empty, 
                        UserId = CurrentUserId, 
                        Month = Month, 
                        Year = 2025 
                    });
                }
            }

            // Load habit entries for current month
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

        public IActionResult OnPostAddHabit()
        {
            Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrWhiteSpace(Username))
                return RedirectToPage(new { Month, Year });

            var user = _db.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null) return RedirectToPage(new { Month, Year });

            if (!string.IsNullOrWhiteSpace(NewHabit))
            {
                // Check if habit already exists for this user in 2025
                var existingHabit = _db.HabitEntries
                    .FirstOrDefault(e => e.UserId == user.Id && e.HabitName == NewHabit && e.Year == 2025);
                
                if (existingHabit == null)
                {
                    // Create habit entries for all days in 2025
                    for (int month = 1; month <= 12; month++)
                    {
                        int daysInMonth = DateTime.DaysInMonth(2025, month);
                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            var entry = new HabitEntry 
                            { 
                                Day = day, 
                                HabitName = NewHabit, 
                                IsCompleted = false, 
                                UserId = user.Id, 
                                Month = month, 
                                Year = 2025 
                            };
                            _db.HabitEntries.Add(entry);
                        }
                    }
                    _db.SaveChanges();
                }
            }
            
            // Redirect back to the same month to show the new habit
            return RedirectToPage(new { Month, Year });
        }

        public IActionResult OnPostDeleteHabit(string habit)
        {
            Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrWhiteSpace(Username))
                return RedirectToPage(new { Month, Year });

            var user = _db.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null) return RedirectToPage(new { Month, Year });

            if (!string.IsNullOrWhiteSpace(habit))
            {
                // Delete all entries for this habit, user, and year 2025
                var entriesToDelete = _db.HabitEntries
                    .Where(e => e.UserId == user.Id && e.HabitName == habit && e.Year == 2025)
                    .ToList();
                
                _db.HabitEntries.RemoveRange(entriesToDelete);
                _db.SaveChanges();
            }
            
            // Redirect back to the same month
            return RedirectToPage(new { Month, Year });
        }

        public IActionResult OnPostLogin()
        {
            if (!string.IsNullOrWhiteSpace(Username))
            {
                HttpContext.Session.SetString("Username", Username);
            }
            return RedirectToPage(new { Month, Year });
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage(new { Month, Year });
        }

        public IActionResult OnPostSave()
        {
            Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrWhiteSpace(Username))
                return RedirectToPage(new { Month, Year });

            var user = _db.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null) return RedirectToPage(new { Month, Year });

            CurrentUserId = user.Id;

            // Get the current month from the form data
            var formMonth = Request.Form["Month"].FirstOrDefault();
            if (!string.IsNullOrEmpty(formMonth) && int.TryParse(formMonth, out int currentMonth))
            {
                Month = currentMonth;
                _logger.LogInformation($"Save: Using month from form: {Month}");
            }
            else
            {
                _logger.LogWarning($"Save: Could not parse month from form. Form value: '{formMonth}'");
            }

            // Defensive: Ensure Notes and HabitChecks are not null
            if (Notes == null)
                Notes = new Dictionary<string, string>();
            if (HabitChecks == null)
                HabitChecks = new Dictionary<string, Dictionary<string, string>>();

            // Save memorable moments
            if (Notes != null)
            {
                _logger.LogInformation($"Save: Processing {Notes.Count} memorable moments");
                foreach (var kv in Notes)
                {
                    if (!int.TryParse(kv.Key, out int day)) continue;
                    string note = kv.Value ?? string.Empty;
                    var moment = _db.MemorableMoments
                        .FirstOrDefault(m => m.UserId == CurrentUserId && m.Day == day && m.Month == Month && m.Year == 2025);
                    if (moment == null)
                    {
                        moment = new MemorableMoment
                        {
                            UserId = CurrentUserId,
                            Day = day,
                            Month = Month,
                            Year = 2025,
                            Note = note
                        };
                        _db.MemorableMoments.Add(moment);
                        _logger.LogInformation($"Save: Created new moment for day {day}, month {Month}");
                    }
                    else
                    {
                        moment.Note = note;
                        _logger.LogInformation($"Save: Updated moment for day {day}, month {Month}");
                    }
                }
            }
            else
            {
                _logger.LogInformation("Save: No memorable moments to process");
            }

            // Save habit completion status
            if (HabitChecks != null)
            {
                _logger.LogInformation($"Save: Processing habit checks for {HabitChecks.Count} days");
                foreach (var dayKvp in HabitChecks)
                {
                    if (!int.TryParse(dayKvp.Key, out int day)) continue;
                    foreach (var habitKvp in dayKvp.Value)
                    {
                        string habit = habitKvp.Key;
                        bool isChecked = habitKvp.Value == "on";
                        var entry = _db.HabitEntries
                            .FirstOrDefault(e => e.UserId == CurrentUserId && e.Day == day && e.HabitName == habit && e.Month == Month && e.Year == 2025);
                        if (entry != null)
                        {
                            entry.IsCompleted = isChecked;
                            _logger.LogInformation($"Save: Updated habit '{habit}' for day {day}, month {Month} to {isChecked}");
                        }
                        else
                        {
                            _logger.LogWarning($"Save: Could not find habit entry for '{habit}' day {day}, month {Month}");
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("Save: No habit checks to process");
            }

            _db.SaveChanges();
            _logger.LogInformation("Save: Database changes saved successfully");
            TempData["Saved"] = true;
            return RedirectToPage(new { Month, Year });
        }
    }
}
