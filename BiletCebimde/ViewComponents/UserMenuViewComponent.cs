using BiletCebimde.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BiletCebimde.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;

        public UserMenuViewComponent(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_signInManager.IsSignedIn(HttpContext.User))
            {
                return View("NotSignedIn");
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return View("NotSignedIn");
            }

            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var isAdmin = userRoles.Contains("Admin");
            var isOrganizer = userRoles.Contains("Organizer");
            var roleBadgeClass = isAdmin ? "bg-danger" : (isOrganizer ? "bg-warning" : "bg-primary");
            var roleDisplayName = isAdmin ? "Admin" : (isOrganizer ? "Organizatör" : "Kullanıcı");

            var model = new UserMenuViewModel
            {
                CurrentUser = currentUser,
                IsAdmin = isAdmin,
                IsOrganizer = isOrganizer,
                RoleBadgeClass = roleBadgeClass,
                RoleDisplayName = roleDisplayName
            };

            return View(model);
        }
    }

    public class UserMenuViewModel
    {
        public Users CurrentUser { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOrganizer { get; set; }
        public string RoleBadgeClass { get; set; } = string.Empty;
        public string RoleDisplayName { get; set; } = string.Empty;
    }
}

