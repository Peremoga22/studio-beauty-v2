using Microsoft.EntityFrameworkCore;

using Telegram.Bot.Types;

using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;
using webStudioBlazor.Interfaces.Contract;

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
                cart.IsActive = false;               
                db.CartItems.RemoveRange(cart.Items);

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

        public async Task<ClientOrders> SaveClientOrderAsync(ClientOrders clientOrders)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
                        
            var entity = await db.ClientOrders
                .AsTracking()
                .SingleOrDefaultAsync(x => x.Id == clientOrders.Id);

            if (entity is null)
            {               
                entity = new ClientOrders
                {
                    ClientFirstName = clientOrders.ClientFirstName?.Trim() ?? string.Empty,
                    ClientLastName = clientOrders.ClientLastName?.Trim() ?? string.Empty,
                    ClientPhone = clientOrders.ClientPhone?.Trim() ?? string.Empty,
                    AppointmentDate = clientOrders.AppointmentDate != default
                        ? clientOrders.AppointmentDate
                        : DateOnly.FromDateTime(DateTime.UtcNow.Date),
                    City = clientOrders.City?.Trim() ?? string.Empty,
                    AddressNewPostOffice = clientOrders.AddressNewPostOffice?.Trim() ?? string.Empty,
                    Price = clientOrders.Price,
                    OrderId = clientOrders.OrderId
                };

                db.ClientOrders.Add(entity);
            }
            //else
            //{                
            //    entity.ClientFirstName = clientOrders.ClientFirstName?.Trim() ?? entity.ClientFirstName;
            //    entity.ClientLastName = clientOrders.ClientLastName?.Trim() ?? entity.ClientLastName;
            //    entity.ClientPhone = clientOrders.ClientPhone?.Trim() ?? entity.ClientPhone;
            //    entity.AppointmentDate = clientOrders.AppointmentDate != default
            //        ? clientOrders.AppointmentDate
            //        : entity.AppointmentDate;
            //    entity.City = clientOrders.City?.Trim() ?? entity.City;
            //    entity.AddressNewPostOffice = clientOrders.AddressNewPostOffice?.Trim() ?? entity.AddressNewPostOffice;
            //    entity.Price = clientOrders.Price != default ? clientOrders.Price : entity.Price;
            //    entity.OrderId = clientOrders.OrderId != default ? clientOrders.OrderId : entity.OrderId;

            //    db.ClientOrders.Update(entity);
            //}

            await db.SaveChangesAsync();
            return entity;
        }


        public async Task<List<Order>> GetAllAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Orders
                .Include(o => o.ClientOrder)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Orders
                .Include(o => o.ClientOrder)
                .Include(o => o.Items).ThenInclude(i => i.Therapy)
                .SingleOrDefaultAsync(o => o.Id == id);
        }
    }
}
