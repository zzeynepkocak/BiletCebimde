using BiletCebimde.Data;
using BiletCebimde.Interfaces;
using BiletCebimde.Models;
using BiletCebimde.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiletCebimde.Controllers
{
    [Authorize]
    public class RegistrationsController : Controller
    {
        private readonly IGenericRepository<Registration> _registrationRepository;
        private readonly IGenericRepository<Event> _eventRepository;
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public RegistrationsController(
            IGenericRepository<Registration> registrationRepository,
            IGenericRepository<Event> eventRepository,
            AppDbContext context,
            UserManager<Users> userManager)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _context = context;
            _userManager = userManager;
        }

        // POST: Registrations/Register/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int eventId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                return NotFound();
            }

            // Kontrol: Zaten kayıtlı mı?
            var existingRegistration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.UserId == currentUserId && r.EventId == eventId);

            if (existingRegistration != null)
            {
                TempData["ErrorMessage"] = "Bu etkinliğe zaten kayıtlısınız.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // Kontrol: Kontenjan dolu mu?
            if (eventEntity.RegisteredCount >= eventEntity.Capacity)
            {
                TempData["ErrorMessage"] = "Bu etkinliğin kontenjanı dolmuştur.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // Kontrol: Etkinlik tarihi geçmiş mi?
            if (eventEntity.Date < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Geçmiş etkinliklere kayıt olamazsınız.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // Kayıt oluştur
            var registration = new Registration
            {
                UserId = currentUserId,
                EventId = eventId,
                RegistrationDate = DateTime.Now
            };

            await _registrationRepository.AddAsync(registration);

            // Etkinliğin kayıt sayısını artır
            eventEntity.RegisteredCount++;
            _context.Events.Update(eventEntity);

            await _registrationRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Etkinliğe başarıyla kayıt oldunuz!";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // POST: Registrations/Unregister/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unregister(int eventId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.UserId == currentUserId && r.EventId == eventId);

            if (registration == null)
            {
                TempData["ErrorMessage"] = "Bu etkinliğe kayıtlı değilsiniz.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // Kaydı sil
            _registrationRepository.Delete(registration);

            // Etkinliğin kayıt sayısını azalt
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity != null)
            {
                eventEntity.RegisteredCount--;
                _context.Events.Update(eventEntity);
            }

            await _registrationRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Etkinlik kaydınız iptal edildi.";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // GET: Registrations/MyRegistrations (Kullanıcının kayıt olduğu etkinlikler)
        public async Task<IActionResult> MyRegistrations()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var registrations = await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Category)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Venue)
                .Include(r => r.Event)
                    .ThenInclude(e => e.Organizer)
                .Where(r => r.UserId == currentUserId)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();

            var viewModels = registrations.Select(r => new RegistrationViewModel
            {
                EventId = r.Event.Id,
                EventTitle = r.Event.Title ?? string.Empty,
                EventDescription = r.Event.Description ?? string.Empty,
                EventDate = r.Event.Date,
                EventCapacity = r.Event.Capacity,
                EventRegisteredCount = r.Event.RegisteredCount,
                CategoryName = r.Event.Category?.Name ?? string.Empty,
                VenueName = r.Event.Venue?.Name ?? string.Empty,
                VenueAddress = r.Event.Venue?.Address ?? string.Empty,
                OrganizerName = r.Event.Organizer?.FullName ?? string.Empty,
                RegistrationDate = r.RegistrationDate
            }).ToList();

            return View(viewModels);
        }

        // POST: Registrations/CancelRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRegistration(int eventId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kaydı bul
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.UserId == currentUserId && r.EventId == eventId);

            if (registration == null)
            {
                TempData["ErrorMessage"] = "Bu etkinliğe kayıtlı değilsiniz.";
                return RedirectToAction("MyRegistrations");
            }

            // Yetki kontrolü: Sadece kaydı oluşturan kullanıcı iptal edebilir
            if (registration.UserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Bu kaydı iptal etme yetkiniz yok.";
                return RedirectToAction("MyRegistrations");
            }

            // Kaydı sil
            _registrationRepository.Delete(registration);

            // Etkinliğin kayıt sayısını azalt
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity != null)
            {
                eventEntity.RegisteredCount--;
                if (eventEntity.RegisteredCount < 0)
                {
                    eventEntity.RegisteredCount = 0;
                }
                _context.Events.Update(eventEntity);
            }

            await _registrationRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Etkinlik kaydınız başarıyla iptal edildi.";
            return RedirectToAction("MyRegistrations");
        }
    }
}

