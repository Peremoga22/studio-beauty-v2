using Microsoft.EntityFrameworkCore;

using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;
using webStudioBlazor.ModelDTOs;

namespace webStudioBlazor.Services
{
    public class GiftCertificateService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public GiftCertificateService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task SaveGiftCertificateAsync(GiftCertificate certificate, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            if (certificate.Id == 0)
            {
                await db.GiftCertificates.AddAsync(certificate, ct);
            }
            else
            {
                db.GiftCertificates.Update(certificate);
            }

            await db.SaveChangesAsync(ct);
        }
        public async Task<List<GiftCertificate>> GetGiftCertificatesByUserIdAsync(string userId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.GiftCertificates
                .AsNoTracking()
                .Where(gc => gc.UserId == userId)
                .OrderByDescending(gc => gc.CreatedAt)
                .ToListAsync(ct);
        }
        public async Task<List<GiftCertificateWithClientDto>> GetGiftCertificatesAll(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.GiftCertificates
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new GiftCertificateWithClientDto
                {
                    Id = c.Id,
                    RecipientName = c.RecipientName,
                    Amount = c.Amount,
                    IsApproved = c.IsApproved,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User != null ? c.User.UserName : string.Empty,

                    ClientName = db.Appointments
                   .Where(a => a.UserId == c.UserId)
                   .OrderByDescending(a => a.AppointmentDate)
                   .Select(a => a.ClientName)
                   .FirstOrDefault(),
                    ClientPhone = db.Appointments
                   .Where(a => a.UserId == c.UserId)
                   .OrderByDescending(a => a.AppointmentDate)
                   .Select(a => a.ClientPhone)
                   .FirstOrDefault()
                })
         .ToListAsync(ct);
        }
        public async Task ApproveGiftCertificateAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var certificate = await db.GiftCertificates.FindAsync(id);
            if (certificate != null)
            {
                certificate.IsApproved = true;
                await db.SaveChangesAsync();
            }
        }

        public async Task<Appointment?> GetCustomerFullNameByUserIdAsync(string userId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var user = await db.Appointments
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(ct);
            return user;
        }
    }
}
