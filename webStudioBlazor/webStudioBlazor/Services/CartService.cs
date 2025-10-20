using Microsoft.EntityFrameworkCore;

using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;
using webStudioBlazor.Interfaces.Contract;

namespace webStudioBlazor.Services
{
    public class CartService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ISessionService _session;
        public event Action<int>? CartCountChanged;     
        private int _notifyInFlight = 0;
        private int _currentCount;

        public CartService(IDbContextFactory<ApplicationDbContext> dbFactory, ISessionService session)
        {
            _dbFactory = dbFactory;
            _session = session;
        }

        public async Task<Cart> GetOrCreateAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var key = _session.GetSessionKey();

            var cart = await db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Therapy)
                .FirstOrDefaultAsync(c => c.SessionKey == key && c.IsActive);

            if (cart is null)
            {
                cart = new Cart { SessionKey = key, CreatedAt = DateTime.UtcNow, IsActive = true };
                db.Carts.Add(cart);
                await db.SaveChangesAsync();
            }
            return cart;
        }
               
        public async Task AddOrIncrementAsync(int therapyId, int qty = 1)
        {
            if (qty < 1) qty = 1;

            await using var db = await _dbFactory.CreateDbContextAsync();
            var key = _session.GetSessionKey();

            var therapy = await db.TherapyCards.AsNoTracking()
                .SingleOrDefaultAsync(t => t.Id == therapyId)
                ?? throw new InvalidOperationException($"TherapyCard Id={therapyId} не знайдено.");

            var cart = await db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.SessionKey == key && c.IsActive);

            if (cart is null)
            {
                cart = new Cart { SessionKey = key, CreatedAt = DateTime.UtcNow, IsActive = true };
                db.Carts.Add(cart);
            }

            var existing = cart.Items.FirstOrDefault(i => i.TherapyId == therapyId);
            if (existing is null)
            {
                cart.Items.Add(new CartItem
                {
                    TherapyId = therapy.Id,
                    UnitPrice = therapy.Price,
                    Quantity = qty
                });
            }
            else
            {
                existing.Quantity += qty;
                db.Entry(existing).State = EntityState.Modified;
            }
                      
            BumpCountLocally(qty);

            await db.SaveChangesAsync();
                       
            await NotifyCountChangedAsync();
        }

        public async Task UpdateQuantityAsync(int itemId, int qty)
        {
            if (qty < 1) qty = 1;
            await using var db = await _dbFactory.CreateDbContextAsync();

            var item = await db.CartItems.FindAsync(itemId);
            if (item is null) return;

            // різниця, щоб коректно підбити бейдж оптимістично
            var delta = qty - item.Quantity;
            item.Quantity = qty;

            // миттєве локальне оновлення
            if (delta != 0) BumpCountLocally(delta);

            await db.SaveChangesAsync();
            await NotifyCountChangedAsync();
        }

        public async Task RemoveItemAsync(int itemId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var item = await db.CartItems.FindAsync(itemId);
            if (item is null) return;

            // зменшуємо бейдж на кількість елемента
            BumpCountLocally(-item.Quantity);

            db.CartItems.Remove(item);
            await db.SaveChangesAsync();
            await NotifyCountChangedAsync();
        }

        public async Task ClearAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var key = _session.GetSessionKey();

            var qtyToRemove = await db.CartItems
                .Where(i => i.Cart.SessionKey == key && i.Cart.IsActive)
                .SumAsync(i => (int?)i.Quantity) ?? 0;

            if (qtyToRemove != 0) BumpCountLocally(-qtyToRemove);

            db.CartItems.RemoveRange(db.CartItems.Where(i => i.Cart.SessionKey == key && i.Cart.IsActive));
            await db.SaveChangesAsync();

            await NotifyCountChangedAsync();
        }

        private async Task<int> CountForCurrentSessionAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var key = _session.GetSessionKey();

            return await db.CartItems
                .Where(i => i.Cart.SessionKey == key && i.Cart.IsActive)
                .SumAsync(i => (int?)i.Quantity) ?? 0;
        }

        public async Task NotifyCountChangedAsync()
        {
            // Debounce/guard від паралельних підрахунків
            if (Interlocked.Exchange(ref _notifyInFlight, 1) == 1) return;
            try
            {
                var count = await CountForCurrentSessionAsync();
                _currentCount = count;
                CartCountChanged?.Invoke(count);
            }
            finally
            {
                Volatile.Write(ref _notifyInFlight, 0);
            }
        }

        private void BumpCountLocally(int delta)
        {
            _currentCount = Math.Max(0, _currentCount + delta);
            CartCountChanged?.Invoke(_currentCount);
        }
        public void IncreaseHeaderCount(int qty)
        {
            if (qty < 1) qty = 1;
            _currentCount += qty;
            CartCountChanged?.Invoke(_currentCount);
        }
    }
}
