using BiletCebimde.Data;
using BiletCebimde.Interfaces;
using BiletCebimde.Models;
using BiletCebimde.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiletCebimde.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VenuesController : Controller
    {
        private readonly IGenericRepository<Venue> _venueRepository;
        private readonly AppDbContext _context;

        public VenuesController(IGenericRepository<Venue> venueRepository, AppDbContext context)
        {
            _venueRepository = venueRepository;
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues
                .Include(v => v.Events)
                .ToListAsync();

            var viewModels = venues.Select(v => new VenueViewModel
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                EventCount = v.Events?.Count ?? 0
            }).ToList();

            return View(viewModels);
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venue == null)
            {
                return NotFound();
            }

            var viewModel = new VenueViewModel
            {
                Id = venue.Id,
                Name = venue.Name,
                Address = venue.Address,
                EventCount = venue.Events?.Count ?? 0
            };

            return View(viewModel);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VenueViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var venue = new Venue
                {
                    Name = viewModel.Name,
                    Address = viewModel.Address
                };

                await _venueRepository.AddAsync(venue);
                await _venueRepository.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Mekan başarıyla oluşturuldu!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            var viewModel = new VenueViewModel
            {
                Id = venue.Id,
                Name = venue.Name,
                Address = venue.Address
            };

            return View(viewModel);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VenueViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var venue = await _venueRepository.GetByIdAsync(id);
                if (venue == null)
                {
                    return NotFound();
                }

                venue.Name = viewModel.Name;
                venue.Address = viewModel.Address;
                _venueRepository.Update(venue);
                await _venueRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Mekan başarıyla güncellendi!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venue == null)
            {
                return NotFound();
            }

            var viewModel = new VenueViewModel
            {
                Id = venue.Id,
                Name = venue.Name,
                Address = venue.Address,
                EventCount = venue.Events?.Count ?? 0
            };

            return View(viewModel);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            // Mekana bağlı etkinlik var mı kontrol et
            var hasEvents = await _context.Events.AnyAsync(e => e.VenueId == id);
            if (hasEvents)
            {
                TempData["ErrorMessage"] = "Bu mekana bağlı etkinlikler bulunduğu için mekan silinemez. Önce etkinlikleri silin veya başka bir mekana taşıyın.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            _venueRepository.Delete(venue);
            await _venueRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mekan başarıyla silindi!";
            return RedirectToAction(nameof(Index));
        }
    }
}

