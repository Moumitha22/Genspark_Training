using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();
        Task<ListerDashboardDto> GetListerDashboardAsync(Guid listerId);
    }
}