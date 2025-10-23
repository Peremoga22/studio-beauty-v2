using Microsoft.EntityFrameworkCore;

using webStudioBlazor.Data;
using webStudioBlazor.Statistics;

namespace webStudioBlazor.Services
{
    public class AnalyticsService
    {
        private readonly ApplicationDbContext _db;

        public AnalyticsService(ApplicationDbContext db) => _db = db;

        public async Task<IReadOnlyList<AnalyticsPoint>> GetAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            static bool IsUnset(DateTime? dt) =>
                !dt.HasValue || dt.Value == DateTime.MinValue || dt.Value.Year <= 1;

            var defaultFrom = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
            var defaultToExclusive = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

            var start = !IsUnset(from) ? DateOnly.FromDateTime(from!.Value.Date) : defaultFrom;
            var endExclusive = !IsUnset(to)
                ? DateOnly.FromDateTime(to!.Value.Date).AddDays(1)
                : defaultToExclusive;

            if (start >= endExclusive)
            {
                var s = DateOnly.FromDateTime((to ?? DateTime.Today).Date);
                var e = DateOnly.FromDateTime((from ?? DateTime.Today).Date).AddDays(1);
                start = s;
                endExclusive = e;
                if (start >= endExclusive)
                    endExclusive = start.AddDays(1);
            }

            // -------- Записи (як було) --------
            var rawAppointments = await (
                from a in _db.Appointments.AsNoTracking()
                join c in _db.Categories.AsNoTracking() on a.CategoryId equals c.Id into gj
                from c in gj.DefaultIfEmpty()
                where a.AppointmentDate >= start && a.AppointmentDate < endExclusive
                select new
                {
                    a.AppointmentDate, // DateOnly
                    a.Price,
                    CategoryName = c != null ? c.NameCategory : null
                }
            ).ToListAsync(ct);

            // Базові точки за записами
            var pointsByDate = rawAppointments
                .GroupBy(x => x.AppointmentDate)
                .OrderBy(g => g.Key)
                .Select(g => new AnalyticsPoint
                {
                    Date = g.Key,
                    CosmetologyCount =
                        g.Count(x => (x.CategoryName ?? string.Empty)
                            .Contains("космет", StringComparison.OrdinalIgnoreCase)),
                    MassageCount =
                        g.Count(x => (x.CategoryName ?? string.Empty)
                            .Contains("масаж", StringComparison.OrdinalIgnoreCase)),
                    CosmetologyRevenue =
                        g.Where(x => (x.CategoryName ?? string.Empty)
                            .Contains("космет", StringComparison.OrdinalIgnoreCase))
                         .Sum(x => x.Price),
                    MassageRevenue =
                        g.Where(x => (x.CategoryName ?? string.Empty)
                            .Contains("масаж", StringComparison.OrdinalIgnoreCase))
                         .Sum(x => x.Price)
                })
                .ToDictionary(p => p.Date, p => p);

            // -------- ✅ Продажі (Orders) --------
            // Беремо замовлення за діапазоном, групуємо по даті
            var rawOrders = await _db.Orders
                .AsNoTracking()
                .Where(o =>
                    DateOnly.FromDateTime(o.OrderDate) >= start &&
                    DateOnly.FromDateTime(o.OrderDate) < endExclusive)
                .Select(o => new
                {
                    Day = DateOnly.FromDateTime(o.OrderDate),
                    o.TotalAmount
                })
                .ToListAsync(ct);

            var ordersGrouped = rawOrders
                .GroupBy(o => o.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    OrdersCount = g.Count(),
                    SalesRevenue = g.Sum(x => x.TotalAmount)
                })
                .ToList();

            // Мердж у вже нараховані точки (або створюємо нові, якщо в цей день записів не було)
            foreach (var g in ordersGrouped)
            {
                if (!pointsByDate.TryGetValue(g.Day, out var p))
                {
                    p = new AnalyticsPoint { Date = g.Day };
                    pointsByDate[g.Day] = p;
                }

                p.OrdersCount += g.OrdersCount;
                p.SalesRevenue += g.SalesRevenue;
            }

            // Повертаємо впорядковано за датою
            return pointsByDate.Values
                .OrderBy(p => p.Date)
                .ToList();
        }


    }
}
