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
    public class CategoriesController : Controller
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly AppDbContext _context;

        public CategoriesController(IGenericRepository<Category> categoryRepository, AppDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Events)
                .ToListAsync();

            var viewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                EventCount = c.Events?.Count ?? 0
            }).ToList();

            return View(viewModels);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Events)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                EventCount = category.Events?.Count ?? 0
            };

            return View(viewModel);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = viewModel.Name
                };

                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(viewModel);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Name = viewModel.Name;
                _categoryRepository.Update(category);
                await _categoryRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Events)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                EventCount = category.Events?.Count ?? 0
            };

            return View(viewModel);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Kategoriye bağlı etkinlik var mı kontrol et
            var hasEvents = await _context.Events.AnyAsync(e => e.CategoryId == id);
            if (hasEvents)
            {
                TempData["ErrorMessage"] = "Bu kategoriye bağlı etkinlikler bulunduğu için kategori silinemez. Önce etkinlikleri silin veya başka bir kategoriye taşıyın.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategori başarıyla silindi!";
            return RedirectToAction(nameof(Index));
        }
    }
}
