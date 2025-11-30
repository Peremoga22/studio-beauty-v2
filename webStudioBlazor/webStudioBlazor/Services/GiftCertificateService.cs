using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;
using webStudioBlazor.ModelDTOs;
using webStudioBlazor.Services.PDF;

namespace webStudioBlazor.Services
{
    public class GiftCertificateService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly IJSRuntime _jsRuntime;

        public GiftCertificateService(IDbContextFactory<ApplicationDbContext> dbFactory,IJSRuntime jsRuntime)
        {
            _dbFactory = dbFactory;
            _jsRuntime = jsRuntime;
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

        public async Task DeleteGiftCertificateAsync(int giftCertificateId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
                        
            var entity = await db.GiftCertificates
                .FirstOrDefaultAsync(x => x.Id == giftCertificateId, ct);
                      
            if (entity is null)
                return;
                     
            db.GiftCertificates.Remove(entity);                       
            await db.SaveChangesAsync(ct);
        }

        public async Task<string> DownloadGiftCertificatePdfAsync(int certificateId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
                       
            var cert = await db.GiftCertificates
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == certificateId);

            if (cert is null)
                return "Сертифікат не знайдено!";
                     
            var pdfBytes = await GenerateGiftCertificatePdfAsync(certificateId);

            if (pdfBytes is null || pdfBytes.Length == 0)
                return "Не вдалося створити PDF-файл!";
                      
            var rawName = cert.RecipientName ?? "Recipient";
                       
            var safeName = string.Concat(
                rawName.Split(Path.GetInvalidFileNameChars())
            ).Replace(" ", "_");
                      
            var fileName = $"Подарунковий сертифікат від {safeName}.pdf";
                        
            await _jsRuntime.InvokeVoidAsync(
                "saveAsFile",
                fileName,
                Convert.ToBase64String(pdfBytes)
            );

            return $"Файл {fileName} успішно завантажено!";
        }

        private async Task<byte[]> GenerateGiftCertificatePdfAsync(int certId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var cert = await db.GiftCertificates
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == certId);

            if (cert is null)
                return null;

            var generator = new GiftCertificatePdfGenerator();
            return generator.Generate(cert);
        }

    }
}
