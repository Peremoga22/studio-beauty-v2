using Microsoft.EntityFrameworkCore;

using System;

using webStudioBlazor.Data;
using webStudioBlazor.ModelDTOs;

namespace webStudioBlazor.Services
{
    public class AdminCustomerOrdersService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public AdminCustomerOrdersService(IDbContextFactory<ApplicationDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<List<CustomerOrderRowDto>> GetCustomerOrdersAsync(
            string? search, DateOnly? from, DateOnly? to, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
                       
            var q =
                from oi in db.OrderItems.AsNoTracking()
                join o in db.Orders.AsNoTracking() on oi.OrderId equals o.Id
                join co in db.ClientOrders.AsNoTracking() on o.Id equals co.OrderId
                join t in db.TherapyCards.AsNoTracking() on oi.TherapyId equals t.Id 
                select new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    co.ClientFirstName,
                    co.ClientLastName,
                    co.ClientPhone,
                    co.City,
                    NewPostOffice = co.AddressNewPostOffice, 
                    AddressLine = co.AddressNewPostOffice, 
                    ItemName = t.TitleCard,
                    oi.Quantity,
                    oi.UnitPrice
                };
                       
            if (from is not null)
            {
                var f = from.Value.ToDateTime(TimeOnly.MinValue);
                q = q.Where(x => x.OrderDate >= f);
            }
            if (to is not null)
            {
                var tmax = to.Value.ToDateTime(TimeOnly.MaxValue);
                q = q.Where(x => x.OrderDate <= tmax);
            }
                        
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(x =>
                    x.ClientFirstName.Contains(s) ||
                    x.ClientLastName.Contains(s) ||
                    x.ClientPhone.Contains(s));
            }

            var list = await q
                .OrderByDescending(x => x.OrderDate)
                .ThenBy(x => x.ClientLastName)
                .ThenBy(x => x.ClientFirstName)
                .ToListAsync(ct);
                        
            var rows = list.Select(x => new CustomerOrderRowDto
            {
                OrderId = x.Id,
                OrderDate = DateOnly.FromDateTime(x.OrderDate.Date),
                OrderTotal = x.TotalAmount,

                ClientFullName = $"{x.ClientLastName} {x.ClientFirstName}".Trim(),
                ClientPhone = x.ClientPhone,
                City = x.City ?? "",
                NewPostOffice = x.NewPostOffice ?? "",
                AddressNewPostOffice = x.AddressLine,

                ItemName = x.ItemName,
                Quantity = Math.Max(1, x.Quantity),
                UnitPrice = x.UnitPrice
            })
            .ToList();

            return rows;
        }
    }
}
