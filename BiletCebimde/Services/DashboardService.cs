using BiletCebimde.Data;
using BiletCebimde.Models;
using BiletCebimde.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BiletCebimde.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetAdminDashboardDataAsync()
        {
            var totalEvents = await _context.Events.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalRegistrations = await _context.Registrations.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalVenues = await _context.Venues.CountAsync();

            // En çok kayıt alan 5 etkinlik
            var topEvents = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .OrderByDescending(e => e.RegisteredCount)
                .Take(5)
                .Select(e => new TopEventViewModel
                {
                    EventId = e.Id,
                    EventTitle = e.Title ?? string.Empty,
                    RegisteredCount = e.RegisteredCount,
                    CategoryName = e.Category != null ? e.Category.Name : string.Empty,
                    VenueName = e.Venue != null ? e.Venue.Name : string.Empty
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalEvents = totalEvents,
                TotalUsers = totalUsers,
                TotalRegistrations = totalRegistrations,
                TotalCategories = totalCategories,
                TotalVenues = totalVenues,
                TopEvents = topEvents
            };
        }

        public async Task<DashboardViewModel> GetOrganizerDashboardDataAsync(string organizerId)
        {
            // Organizatörün oluşturduğu etkinlikler
            var organizerEvents = await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();

            var totalEvents = organizerEvents.Count;

            // Bu etkinliklere yapılan toplam kayıt sayısı
            var totalRegistrations = organizerEvents.Sum(e => e.RegisteredCount);

            // Include ile ilişkili verileri yükle ve en çok kayıt alan 3 etkinlik
            var eventsWithIncludes = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.RegisteredCount)
                .Take(3)
                .ToListAsync();

            var topEventsWithDetails = eventsWithIncludes.Select(e => new TopEventViewModel
            {
                EventId = e.Id,
                EventTitle = e.Title ?? string.Empty,
                RegisteredCount = e.RegisteredCount,
                CategoryName = e.Category?.Name ?? string.Empty,
                VenueName = e.Venue?.Name ?? string.Empty
            }).ToList();

            return new DashboardViewModel
            {
                TotalEvents = totalEvents,
                TotalRegistrations = totalRegistrations,
                TopEvents = topEventsWithDetails
            };
        }

        public async Task<DashboardViewModel> GetUserDashboardDataAsync(string userId)
        {
            // Kullanıcının kayıt olduğu etkinlik sayısı
            var totalRegistrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .CountAsync();

            // En son katıldığı etkinlik
            var lastRegistration = await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Category)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Venue)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RegistrationDate)
                .FirstOrDefaultAsync();

            var lastEvent = lastRegistration?.Event;

            return new DashboardViewModel
            {
                TotalRegistrations = totalRegistrations,
                LastEvent = (lastEvent != null && lastRegistration != null) ? new LastEventViewModel
                {
                    EventId = lastEvent.Id,
                    EventTitle = lastEvent.Title ?? string.Empty,
                    EventDate = lastEvent.Date,
                    CategoryName = lastEvent.Category?.Name ?? string.Empty,
                    VenueName = lastEvent.Venue?.Name ?? string.Empty,
                    RegistrationDate = lastRegistration.RegistrationDate
                } : null
            };
        }
    }
}

