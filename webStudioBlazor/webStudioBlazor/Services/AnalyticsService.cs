using Microsoft.EntityFrameworkCore;

using webStudioBlazor.Data;
using webStudioBlazor.Statistics;

namespace webStudioBlazor.Services
{
    public class AnalyticsService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public AnalyticsService(IDbContextFactory<ApplicationDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<AnalyticsPoint>> GetAsync(
            DateTime? from,
            DateTime? to,
            CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            static bool IsUnset(DateTime? dt) =>
                !dt.HasValue || dt.Value == DateTime.MinValue || dt.Value.Year <= 1;

            var today = DateTime.Today;

            var defaultFrom = DateOnly.FromDateTime(today.AddMonths(-1));
            // якщо "to" не заданий — логічніше брати до "сьогодні+1", але лишаю твою ідею діапазону
            var defaultToExclusive = DateOnly.FromDateTime(today.AddDays(31)).AddDays(1);

            var start = !IsUnset(from) ? DateOnly.FromDateTime(from!.Value.Date) : defaultFrom;
            var endExclusive = !IsUnset(to)
                ? DateOnly.FromDateTime(to!.Value.Date).AddDays(1)
                : defaultToExclusive;

            if (start >= endExclusive)
            {
                var s = DateOnly.FromDateTime((to ?? today).Date);
                var e = DateOnly.FromDateTime((from ?? today).Date).AddDays(1);
                if (s >= e) e = s.AddDays(1);
                start = s;
                endExclusive = e;
            }

            static bool IsCosmetology(string? s) =>
                !string.IsNullOrWhiteSpace(s) &&
                (s.Contains("космет", StringComparison.OrdinalIgnoreCase) ||
                 s.Contains("cosmet", StringComparison.OrdinalIgnoreCase));

            static bool IsMassage(string? s) =>
                !string.IsNullOrWhiteSpace(s) &&
                (s.Contains("масаж", StringComparison.OrdinalIgnoreCase) ||
                 s.Contains("massage", StringComparison.OrdinalIgnoreCase));

            static decimal Safe(decimal? v) => v ?? 0m;

            // ✅ ВАЖЛИВО: endExclusive має бути EXCLUSIVE, тому < endExclusive
            var raw = await (
                from a in db.Appointments.AsNoTracking()
                join c in db.Categories.AsNoTracking() on a.CategoryId equals c.Id
                where a.AppointmentDate >= start && a.AppointmentDate < endExclusive
                select new
                {
                    a.AppointmentDate,
                    a.Price,
                    CategoryName = c.NameCategory
                })
                .ToListAsync(ct);

            var pointsByDate = raw
                .Select(x => new
                {
                    x.AppointmentDate,
                    x.Price,
                    IsCos = IsCosmetology(x.CategoryName),
                    IsMas = IsMassage(x.CategoryName)
                })
                .GroupBy(x => x.AppointmentDate)
                .OrderBy(g => g.Key)
                .Select(g => new AnalyticsPoint
                {
                    Date = g.Key,
                    CosmetologyCount = g.Count(x => x.IsCos),
                    MassageCount = g.Count(x => x.IsMas),
                    CosmetologyRevenue = g.Where(x => x.IsCos).Sum(x => Safe(x.Price)),
                    MassageRevenue = g.Where(x => x.IsMas).Sum(x => Safe(x.Price)),
                    OrdersCount = 0,
                    SalesRevenue = 0m
                })
                .ToDictionary(p => p.Date, p => p);

            // ✅ Orders: краще фільтрувати по DateTime діапазону (менше шансів на проблеми з SQL/індексами)
            var startDt = start.ToDateTime(TimeOnly.MinValue);
            var endDt = endExclusive.ToDateTime(TimeOnly.MinValue);

            var rawOrders = await db.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= startDt && o.OrderDate < endDt)
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
                });

            foreach (var g in ordersGrouped)
            {
                if (!pointsByDate.TryGetValue(g.Day, out var p))
                {
                    p = new AnalyticsPoint
                    {
                        Date = g.Day,
                        CosmetologyCount = 0,
                        MassageCount = 0,
                        CosmetologyRevenue = 0m,
                        MassageRevenue = 0m,
                        OrdersCount = 0,
                        SalesRevenue = 0m
                    };
                    pointsByDate[g.Day] = p;
                }

                p.OrdersCount += g.OrdersCount;
                p.SalesRevenue += g.SalesRevenue;
            }

            return pointsByDate.Values
                .OrderBy(p => p.Date)
                .ToList();
        }
    }
}
