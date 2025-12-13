using System;

namespace BiletCebimde.ViewModels
{
    public class RegistrationViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int EventCapacity { get; set; }
        public int EventRegisteredCount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string VenueAddress { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsPastEvent => EventDate < DateTime.Now;
    }
}

