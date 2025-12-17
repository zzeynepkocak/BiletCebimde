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
    public class EventsController : Controller
    {
        private readonly IGenericRepository<Event> _eventRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Venue> _venueRepository;
        private readonly IGenericRepository<Registration> _registrationRepository;
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public EventsController(
            IGenericRepository<Event> eventRepository,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Venue> venueRepository,
            IGenericRepository<Registration> registrationRepository,
            AppDbContext context,
            UserManager<Users> userManager)
        {
            _eventRepository = eventRepository;
            _categoryRepository = categoryRepository;
            _venueRepository = venueRepository;
            _registrationRepository = registrationRepository;
            _context = context;
            _userManager = userManager;
        }

        // Helper method: Etkinlik başlığına göre resim yolunu belirler
        private string GetEventImageUrl(string title)
        {
            if (string.IsNullOrEmpty(title))
                return "/image/default.png";

            var titleLower = title.ToLowerInvariant();

            // Etkinlik başlıklarına göre resim eşleştirmesi
            if (titleLower.Contains("cem yılmaz") || titleLower.Contains("cem yilmaz"))
                return "/image/cemyılmaz.png";
            else if (titleLower.Contains("duman"))
                return "/image/duman.png";
            else if (titleLower.Contains("jazz") || titleLower.Contains("caz"))
                return "/image/jazz.png";
            else if (titleLower.Contains("klasik müzik") || titleLower.Contains("klasik muzik") || titleLower.Contains("senfoni"))
                return "/image/klasikmuzik.png";
            else if (titleLower.Contains("kral lear") || titleLower.Contains("kral lear"))
                return "/image/krallear.png";
            else if (titleLower.Contains("pamuk prenses") || titleLower.Contains("pamuk prenses"))
                return "/image/pamuk prenses.png";
            else if (titleLower.Contains("kuğu gölü") || titleLower.Contains("kugu golu") || titleLower.Contains("swan"))
                return "/image/swan.png";
            else if (titleLower.Contains("traviata") || titleLower.Contains("opera"))
                return "/image/klasikmuzik.png"; // Opera için varsayılan resim
            else
                return "/image/default.png"; // Varsayılan resim (eğer yoksa boş gösterir)
        }

        // GET: Events (Public - Herkes görebilir)
        public async Task<IActionResult> Index(int? categoryId, string? searchString)
        {
            var events = await _eventRepository.GetAllAsync();
            var eventsList = events.ToList();

            // Filtreleme
            if (categoryId.HasValue)
            {
                eventsList = eventsList.Where(e => e.CategoryId == categoryId.Value).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                eventsList = eventsList.Where(e => e.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                                   e.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sadece gelecek etkinlikleri göster
            eventsList = eventsList.Where(e => e.Date >= DateTime.Now).OrderBy(e => e.Date).ToList();

            var currentUserId = _userManager.GetUserId(User);
            var userRegistrations = currentUserId != null
                ? await _context.Registrations.Where(r => r.UserId == currentUserId).Select(r => r.EventId).ToListAsync()
                : new List<int>();

            var viewModels = eventsList.Select(e => new EventViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Capacity = e.Capacity,
                RegisteredCount = e.RegisteredCount,
                CategoryName = e.Category?.Name,
                VenueName = e.Venue?.Name,
                OrganizerName = e.Organizer?.FullName,
                IsRegistered = userRegistrations.Contains(e.Id),
                CanRegister = e.Date >= DateTime.Now && e.RegisteredCount < e.Capacity && !userRegistrations.Contains(e.Id),
                ImageUrl = GetEventImageUrl(e.Title)
            }).ToList();

            ViewBag.Categories = (await _categoryRepository.GetAllAsync())
                .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList();
            ViewBag.CategoryId = categoryId;
            ViewBag.SearchString = searchString;

            return View(viewModels);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // İlişkili verileri (Category, Venue, Organizer) include ederek getir
            var eventEntity = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var isRegistered = currentUserId != null && await _context.Registrations
                .AnyAsync(r => r.UserId == currentUserId && r.EventId == id);

            var viewModel = new EventViewModel
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                Date = eventEntity.Date,
                Capacity = eventEntity.Capacity,
                RegisteredCount = eventEntity.RegisteredCount,
                CategoryName = eventEntity.Category?.Name,
                VenueName = eventEntity.Venue?.Name,
                OrganizerName = eventEntity.Organizer?.FullName,
                IsRegistered = isRegistered,
                CanRegister = eventEntity.Date >= DateTime.Now && eventEntity.RegisteredCount < eventEntity.Capacity && !isRegistered,
                ImageUrl = GetEventImageUrl(eventEntity.Title)
            };

            return View(viewModel);
        }

        // GET: Events/Create (Sadece Organizer ve Admin)
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateEventViewModel
            {
                Categories = (await _categoryRepository.GetAllAsync())
                    .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList(),
                Venues = (await _venueRepository.GetAllAsync())
                    .Select(v => new VenueViewModel { Id = v.Id, Name = v.Name }).ToList()
            };

            return View(viewModel);
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create(CreateEventViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Tarih ve saati birleştir
                var eventDateTime = viewModel.Date.Date.Add(viewModel.Time);

                // Geçmiş tarih kontrolü
                if (eventDateTime < DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Etkinlik tarihi geçmiş bir tarih olamaz.");
                    viewModel.Categories = (await _categoryRepository.GetAllAsync())
                        .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList();
                    viewModel.Venues = (await _venueRepository.GetAllAsync())
                        .Select(v => new VenueViewModel { Id = v.Id, Name = v.Name }).ToList();
                    return View(viewModel);
                }

                var eventEntity = new Event
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Date = eventDateTime,
                    Capacity = viewModel.Capacity,
                    CategoryId = viewModel.CategoryId,
                    VenueId = viewModel.VenueId,
                    OrganizerId = currentUserId,
                    RegisteredCount = 0
                };

                await _eventRepository.AddAsync(eventEntity);
                await _eventRepository.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Etkinlik başarıyla oluşturuldu!";
                return RedirectToAction(nameof(Details), new { id = eventEntity.Id });
            }

            // Model geçersizse dropdown listeleri yeniden yükle
            viewModel.Categories = (await _categoryRepository.GetAllAsync())
                .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList();
            viewModel.Venues = (await _venueRepository.GetAllAsync())
                .Select(v => new VenueViewModel { Id = v.Id, Name = v.Name }).ToList();

            return View(viewModel);
        }

        // GET: Events/Edit/5 (Sadece Organizer kendi etkinliklerini, Admin hepsini düzenleyebilir)
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int id)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Organizer sadece kendi etkinliklerini düzenleyebilir
            if (!isAdmin && eventEntity.OrganizerId != currentUserId)
            {
                return Forbid();
            }

            var viewModel = new EditEventViewModel
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                Date = eventEntity.Date.Date,
                Time = eventEntity.Date.TimeOfDay,
                Capacity = eventEntity.Capacity,
                CategoryId = eventEntity.CategoryId,
                VenueId = eventEntity.VenueId,
                Categories = (await _categoryRepository.GetAllAsync())
                    .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList(),
                Venues = (await _venueRepository.GetAllAsync())
                    .Select(v => new VenueViewModel { Id = v.Id, Name = v.Name }).ToList()
            };

            return View(viewModel);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int id, EditEventViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Organizer sadece kendi etkinliklerini düzenleyebilir
            if (!isAdmin && eventEntity.OrganizerId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Tarih ve saati birleştir
                var eventDateTime = viewModel.Date.Date.Add(viewModel.Time);

                // Geçmiş tarih kontrolü (eğer tarih değiştirildiyse)
                if (eventDateTime < DateTime.Now && eventDateTime != eventEntity.Date)
                {
                    ModelState.AddModelError("Date", "Etkinlik tarihi geçmiş bir tarih olamaz.");
                    viewModel.Categories = (await _categoryRepository.GetAllAsync())
                        .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList();
                    viewModel.Venues = (await _venueRepository.GetAllAsync())
                        .Select(v => new VenueViewModel { Id = v.Id, Name = v.Name }).ToList();
                    return View(viewModel);
                }

                eventEntity.Title = viewModel.Title;
                eventEntity.Description = viewModel.Description;
                eventEntity.Date = eventDateTime;
                eventEntity.Capacity = viewModel.Capacity;
                eventEntity.CategoryId = viewModel.CategoryId;
                eventEntity.VenueId = viewModel.VenueId;

                _eventRepository.Update(eventEntity);
                await _eventRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Etkinlik başarıyla güncellendi!";
                return RedirectToAction(nameof(Details), new { id = eventEntity.Id });
            }

            viewModel.Categories = (await _categoryRepository.GetAllAsync())
                .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList();
            viewModel.Venues = (await _venueRepository.GetAllAsync())
                .Select(v => new VenueViewModel { Id = v.Id, Name = v.Name }).ToList();

            return View(viewModel);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Delete(int id)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Organizer sadece kendi etkinliklerini silebilir
            if (!isAdmin && eventEntity.OrganizerId != currentUserId)
            {
                return Forbid();
            }

            var viewModel = new EventViewModel
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                Date = eventEntity.Date,
                Capacity = eventEntity.Capacity,
                RegisteredCount = eventEntity.RegisteredCount,
                CategoryName = eventEntity.Category?.Name,
                VenueName = eventEntity.Venue?.Name,
                OrganizerName = eventEntity.Organizer?.FullName,
                ImageUrl = GetEventImageUrl(eventEntity.Title)
            };

            return View(viewModel);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Organizer sadece kendi etkinliklerini silebilir
            if (!isAdmin && eventEntity.OrganizerId != currentUserId)
            {
                return Forbid();
            }

            _eventRepository.Delete(eventEntity);
            await _eventRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Etkinlik başarıyla silindi!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/MyEvents (Organizer için kendi etkinlikleri)
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> MyEvents()
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            var eventsQuery = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .AsQueryable();

            if (!isAdmin)
            {
                eventsQuery = eventsQuery.Where(e => e.OrganizerId == currentUserId);
            }

            var eventsList = await eventsQuery.OrderByDescending(e => e.Date).ToListAsync();

            var viewModels = eventsList.Select(e => new EventViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Capacity = e.Capacity,
                RegisteredCount = e.RegisteredCount,
                CategoryName = e.Category?.Name,
                VenueName = e.Venue?.Name
            }).OrderByDescending(e => e.Date).ToList();

            return View(viewModels);
        }

        // GET: Events/Participants/5 (Organizer için katılanlar listesi)
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Participants(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Organizer sadece kendi etkinliğinin katılanlarını görebilir
            if (!isAdmin && eventEntity.OrganizerId != currentUserId)
            {
                return Forbid();
            }

            var registrations = await _context.Registrations
                .Include(r => r.User)
                .Where(r => r.EventId == id)
                .ToListAsync();

            ViewBag.EventTitle = eventEntity.Title;
            ViewBag.EventId = id;

            return View(registrations);
        }
    }
}

