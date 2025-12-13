using System;

namespace BiletCebimde.ViewModels
{
    public class DashboardViewModel
    {
        // Admin İstatistikleri
        public int TotalEvents { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalCategories { get; set; }
        public int TotalVenues { get; set; }

        // Organizer İstatistikleri
        public int OrganizerTotalEvents { get; set; }
        public int OrganizerTotalRegistrations { get; set; }

        // User İstatistikleri
        public int UserTotalRegistrations { get; set; }
        public LastEventViewModel? LastEvent { get; set; }

        // Ortak
        public List<TopEventViewModel> TopEvents { get; set; } = new List<TopEventViewModel>();
    }

    public class TopEventViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int RegisteredCount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
    }

    public class LastEventViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
    }
}

