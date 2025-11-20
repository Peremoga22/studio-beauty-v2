using Microsoft.EntityFrameworkCore;

using Telegram.Bot.Types;

using webStudioBlazor.Components.Layout;
using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;
using webStudioBlazor.Interfaces.Contract;
using webStudioBlazor.ModelDTOs;

using static webStudioBlazor.Components.Account.Admin.AdminOrderClients;

namespace webStudioBlazor.Services
{
    public class OrderService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ISessionService _session;
        private readonly CartService _cartService;

        public OrderService(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            ISessionService session,
            CartService cartService)
        {
            _dbFactory = dbFactory;
            _session = session;
            _cartService = cartService;
        }

        public async Task<Order> CreateFromCartAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var key = _session.GetSessionKey();
                      
            
            var cart = await db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Therapy)
                .SingleOrDefaultAsync(c => c.SessionKey == key && c.IsActive);

            if (cart is null || cart.Items.Count == 0)
                throw new InvalidOperationException("Корзина порожня.");
                       
            foreach (var it in cart.Items)
            {
                if (it.UnitPrice <= 0m && it.Therapy is not null)
                    it.UnitPrice = it.Therapy.Price; 
            }

            var total = cart.Items.Sum(i => i.UnitPrice * i.Quantity);                        
            await using var tx = await db.Database.BeginTransactionAsync();

            try
            {                
                var order = new Order
                {
                    SessionKey = key,
                    TotalAmount = total,
                    PaymentStatus = "Pending",
                    OrderStatus = "New",
                    Items = cart.Items.Select(ci => new OrderItem
                    {
                        TherapyId = ci.TherapyId,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice
                    }).ToList()
                };                                               

                db.Orders.Add(order);                            
               
                await db.SaveChangesAsync();
                await tx.CommitAsync();

                return await db.Orders.SingleAsync(o => o.Id == order.Id);                 
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

       public async Task<Order> SaveClientOrderAsync(ClientOrders model)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var sessionKey = _session.GetSessionKey();

            var stagedItems = await db.OrderItems
                .Where(oi => oi.OrderId == 0 && oi.SessionKey == sessionKey)
                .ToListAsync();

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                PaymentStatus = "Pending",
                OrderStatus = "New",
                TotalAmount = 0m,
                SessionKey = sessionKey,
            };
            db.Orders.Add(order);
            await db.SaveChangesAsync();
                       
            var client = new ClientOrders
            {
                OrderId = order.Id,
                ClientFirstName = (model.ClientFirstName ?? string.Empty).Trim(),
                ClientLastName = (model.ClientLastName ?? string.Empty).Trim(),
                ClientPhone = (model.ClientPhone ?? string.Empty).Trim(),
                City = (model.City ?? string.Empty).Trim(),
                AddressNewPostOffice = (model.AddressNewPostOffice ?? string.Empty).Trim(),
                AppointmentDate = model.AppointmentDate != default
                                    ? model.AppointmentDate
                                    : DateOnly.FromDateTime(DateTime.UtcNow.Date),
                Price = 0m,
                UserId = model.UserId
            };
            db.ClientOrders.Add(client);

            order.ClientOrder = client;
            await db.SaveChangesAsync();
                      
            if (stagedItems.Count > 0)
            {
                var byTherapy = stagedItems
                    .GroupBy(i => i.TherapyId)
                    .Select(g => new
                    {
                        First = g.First(),
                        Quantity = g.Sum(x => Math.Max(1, x.Quantity))
                    });

                foreach (var g in byTherapy)
                {
                    var item = g.First;
                    item.OrderId = order.Id;
                    item.Quantity = g.Quantity;
                    item.IsShownInOrder = true;
                }

                var duplicates = stagedItems
                    .GroupBy(i => i.TherapyId)
                    .SelectMany(g => g.Skip(1))
                    .ToList();

                if (duplicates.Count > 0)
                    db.OrderItems.RemoveRange(duplicates);

                await db.SaveChangesAsync();
            }
            else
            {
                var cart = await db.Carts
                    .Include(c => c.Items)
                    .SingleOrDefaultAsync(c => c.SessionKey == sessionKey && c.IsActive);

                var grouped = cart.Items
                    .GroupBy(ci => ci.TherapyId)
                    .Select(g => new
                    {
                        TherapyId = g.Key,
                        Quantity = g.Sum(x => Math.Max(1, x.Quantity)),
                        UnitPrice = g.OrderByDescending(x => x.TotalPrice).First().UnitPrice
                    })
                    .ToList();

                foreach (var g in grouped)
                {
                    if (g.TherapyId == 0) continue;

                    db.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        TherapyId = g.TherapyId,
                        UnitPrice = g.UnitPrice,
                        Quantity = g.Quantity,
                        IsShownInOrder = true
                    });
                }
                await db.SaveChangesAsync();

                if (cart.Items.Count > 0) db.CartItems.RemoveRange(cart.Items);
                cart.IsActive = false;
                db.Carts.Remove(cart);
                await db.SaveChangesAsync();
            }

            order.TotalAmount = await db.OrderItems
                .Where(i => i.OrderId == order.Id)
                .SumAsync(i => i.UnitPrice * i.Quantity);

            client.Price = order.TotalAmount;
            await db.SaveChangesAsync();

            var leftoverCart = await db.Carts
                .Include(c => c.Items)
                .SingleOrDefaultAsync(c => c.SessionKey == sessionKey && c.IsActive);

            if (leftoverCart is not null)
            {
                if (leftoverCart.Items?.Count > 0)
                    db.CartItems.RemoveRange(leftoverCart.Items);

                leftoverCart.IsActive = false;
                db.Carts.Remove(leftoverCart);
                await db.SaveChangesAsync();
            }

            var result = await db.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Therapy)
                .Include(o => o.ClientOrder)
                .FirstAsync(o => o.Id == order.Id);

            await tx.CommitAsync();
            return result;
        }

        //public async Task<List<Order>> GetAllAsync()
        //{
        //    await using var db = await _dbFactory.CreateDbContextAsync();
        //    return await db.Orders
        //        .Include(o => o.ClientOrder)
        //        .OrderByDescending(o => o.OrderDate)
        //        .ToListAsync();
        //}

        //public async Task<Order?> GetByIdAsync(int id)
        //{
        //    await using var db = await _dbFactory.CreateDbContextAsync();
        //    return await db.Orders
        //        .Include(o => o.ClientOrder)
        //        .Include(o => o.Items).ThenInclude(i => i.Therapy)
        //        .SingleOrDefaultAsync(o => o.Id == id);
        //}

        public async Task<ClientOrders?> GetClientOrderByIdAsync(int orderId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.ClientOrders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        public async Task<List<OrderRow>> ListCardsAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var list = await db.OrderItems
                .AsNoTracking()
                .Include(i => i.Order)
                    .ThenInclude(o => o.ClientOrder)
                .Include(i => i.Therapy)
                .OrderByDescending(i => i.Order.OrderDate)
                .Select(i => new OrderRow
                {
                    OrderItemId = i.Id,
                    OrderId = i.OrderId,
                    OrderDate = i.Order.OrderDate,

                    FirstName = i.Order.ClientOrder != null ? i.Order.ClientOrder.ClientFirstName : string.Empty,
                    LastName = i.Order.ClientOrder != null ? i.Order.ClientOrder.ClientLastName : string.Empty,
                    Phone = i.Order.ClientOrder != null ? i.Order.ClientOrder.ClientPhone : string.Empty,

                    City = i.Order.ClientOrder != null ? i.Order.ClientOrder.City : string.Empty,
                    NewPostOffice = i.Order.ClientOrder != null ? i.Order.ClientOrder.AddressNewPostOffice : string.Empty,

                    ItemName = i.Therapy != null ? (i.Therapy.TitleCard ?? string.Empty) : string.Empty,
                                       
                    Quantity = (i.Quantity == 0 ? 1 : i.Quantity),
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.UnitPrice * (decimal)(i.Quantity == 0 ? 1 : i.Quantity),
                                       
                    MoreItemsCount = Math.Max(0, i.Order.Items.Count - 1),
                    IsShownInOrder = i.IsShownInOrder,
                    OrderTotal = i.Order.Items
                        .Select(x => x.UnitPrice * (decimal)(x.Quantity == 0 ? 1 : x.Quantity))
                        .Sum()
                })
                .ToListAsync();

            return list;
        }

        public async Task DeleteOrderAsync(int orderItemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var item = await db.OrderItems.FindAsync(orderItemId);
            if (item != null)
            {
                item.IsShownInOrder = false;
                db.OrderItems.Update(item);
               
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteClientOrderAsync(int orderId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            await using var tx = await db.Database.BeginTransactionAsync(ct);
                       
            var co = await db.ClientOrders
                .FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
                       
            if (co is null)
            {
                await tx.RollbackAsync(ct);
                return;
            }

            var items = await db.OrderItems
                .Where(x => x.OrderId == orderId)
                .ToListAsync(ct);

            if (items.Count > 0)
                db.OrderItems.RemoveRange(items);
                        
            var order = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId, ct);
                      
            db.ClientOrders.Remove(co);
                       
            if (order is not null)
                db.Orders.Remove(order);
                     
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }               
    }
}
