using BiletCebimde.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BiletCebimde.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<Models.Users> _userManager;

        public DashboardController(IDashboardService dashboardService, UserManager<Models.Users> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUserId = currentUser.Id;
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var viewModel = new ViewModels.DashboardViewModel();

            if (userRoles.Contains("Admin"))
            {
                viewModel = await _dashboardService.GetAdminDashboardDataAsync();
            }
            else if (userRoles.Contains("Organizer"))
            {
                viewModel = await _dashboardService.GetOrganizerDashboardDataAsync(currentUserId);
            }
            else
            {
                viewModel = await _dashboardService.GetUserDashboardDataAsync(currentUserId);
            }

            return View(viewModel);
        }
    }
}

