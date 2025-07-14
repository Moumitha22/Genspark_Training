using PropFinderApi.Contexts;
using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace PropFinderApi.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly PropFinderDbContext _context;

        public DashboardService(PropFinderDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProperties = await _context.Properties.CountAsync();
            var totalInquiries = await _context.ContactLogs.CountAsync();

            var totalActiveListers = await _context.Properties
                .Select(p => p.ListerId)
                .Distinct()
                .CountAsync();


            var typeChart = await _context.Properties
                .GroupBy(p => p.PropertyType)
                .Select(g => new ChartItemDto
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync();

            var purposeChart = await _context.Properties
                .GroupBy(p => p.ListingPurpose)
                .Select(g => new ChartItemDto
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync();

            var statusChart = await _context.Properties
                .GroupBy(p => p.Status)
                .Select(g => new ChartItemDto
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync();

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalProperties = totalProperties,
                TotalInquiries = totalInquiries,
                TotalActiveListers = totalActiveListers, 
                PropertyTypeChart = typeChart,
                PropertyPurposeChart = purposeChart,
                PropertyStatusChart = statusChart
            };
        }

        public async Task<ListerDashboardDto> GetListerDashboardAsync(Guid listerId)
        {
            var properties = _context.Properties.Where(p => p.ListerId == listerId);

            var totalListed = await properties.CountAsync();
            var totalForSale = await properties.CountAsync(p => p.ListingPurpose == ListingPurpose.Sale);
            var totalForRent = await properties.CountAsync(p => p.ListingPurpose == ListingPurpose.Rent);
            var totalSoldOut = await properties.CountAsync(p => p.Status == ListingStatus.Sold);
            var totalRented = await properties.CountAsync(p => p.Status == ListingStatus.Rented);
            var totalAvailable = await properties.CountAsync(p => p.Status == ListingStatus.Available);

            var totalInquiries = await _context.ContactLogs
                .CountAsync(c => c.Property.ListerId == listerId);

            var typeChart = await properties
                .GroupBy(p => p.PropertyType)
                .Select(g => new ChartItemDto
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync();

            var purposeChart = await properties
                .GroupBy(p => p.ListingPurpose)
                .Select(g => new ChartItemDto
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync();

            var statusChart = await properties
                .GroupBy(p => p.Status)
                .Select(g => new ChartItemDto
                {
                    Label = g.Key.ToString(),
                    Value = g.Count()
                })
                .ToListAsync();

            return new ListerDashboardDto
            {
                TotalPropertiesListed = totalListed,
                TotalForSale = totalForSale,
                TotalForRent = totalForRent,
                TotalSoldOut = totalSoldOut,
                TotalRented = totalRented,
                TotalAvailable = totalAvailable,
                TotalInquiriesReceived = totalInquiries,
                PropertyTypeChart = typeChart,
                PropertyPurposeChart = purposeChart,
                PropertyStatusChart = statusChart
            };
        }
    }
}
