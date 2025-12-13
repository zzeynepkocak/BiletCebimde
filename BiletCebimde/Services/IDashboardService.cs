using BiletCebimde.ViewModels;

namespace BiletCebimde.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetAdminDashboardDataAsync();
        Task<DashboardViewModel> GetOrganizerDashboardDataAsync(string organizerId);
        Task<DashboardViewModel> GetUserDashboardDataAsync(string userId);
    }
}

