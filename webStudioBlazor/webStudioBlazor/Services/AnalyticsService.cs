using Microsoft.EntityFrameworkCore;

using webStudioBlazor.Data;
using webStudioBlazor.Statistics;

namespace webStudioBlazor.Services
{
    public class AnalyticsService
    {
        private readonly ApplicationDbContext _db;

        public AnalyticsService(ApplicationDbContext db) => _db = db;

        public async Task<IReadOnlyList<AnalyticsPoint>> GetAsync(DateTime? from, DateTime? to, string bucket = "day", CancellationToken ct = default)
        {
            var start = (DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)));
            var end = (DateOnly.FromDateTime(DateTime.Today));

            var startDateTime = start.ToDateTime(TimeOnly.MinValue);
            var endDateTime = end.ToDateTime(TimeOnly.MaxValue);


            static DateTime TruncateToBucket(DateTime d, string b) => b switch
            {
                "week" => StartOfIsoWeek(d),
                "month" => new DateTime(d.Year, d.Month, 1),
                _ => d.Date
            };

            static DateTime StartOfIsoWeek(DateTime date)
            {                
                int diff = (7 + ((int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek) - (int)DayOfWeek.Monday) % 7;
                return date.Date.AddDays(-diff);
            }

            var raw = await (
              from a in _db.Appointments.AsNoTracking()
              join c in _db.Categories.AsNoTracking() on a.CategoryId equals c.Id
              where a.AppointmentDate >= start && a.AppointmentDate <= end
              select new
              {
                  a.AppointmentDate,
                  a.Price,
                  CategoryName = c.NameCategory
              })
              .ToListAsync(ct);

            // Нормалізуємо флаги категорій (чутливість до регістру вимкнено)
            var points = raw
                .Select(x => new
                {                  
                    Bucket = TruncateToBucket(x.AppointmentDate.ToDateTime(TimeOnly.MinValue), bucket),                   
                    IsCosmo = x.CategoryName?.Contains("космет", StringComparison.OrdinalIgnoreCase) == true,
                    IsMassage = x.CategoryName?.Contains("масаж", StringComparison.OrdinalIgnoreCase) == true,
                    Price = x.Price
                })
                .GroupBy(x => x.Bucket)
                .OrderBy(g => g.Key)
                .Select(g => new AnalyticsPoint                                 
                {
                    Date = DateOnly.FromDateTime(g.Key),
                    CosmetologyCount = g.Count(x => x.IsCosmo),
                    MassageCount = g.Count(x => x.IsMassage),
                    CosmetologyRevenue = g.Where(x => x.IsCosmo).Sum(x => x.Price),
                    MassageRevenue = g.Where(x => x.IsMassage).Sum(x => x.Price)
                })
                .ToList();

            return points;
        }
    }
}
