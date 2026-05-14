using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;

namespace webStudioBlazor.Services
{
    public class TreatmentHistoryService
    {
        /// <summary>Максимальний розмір вхідного файлу з браузера (до стиснення).</summary>
        public const int MaxTreatmentImageUploadBytes = 25 * 1024 * 1024;

        private const int StoredImageMaxEdge = 1920;
        private const int StoredJpegQuality = 82;
        private const int PreviewImageMaxEdge = 480;
        private const int PreviewJpegQuality = 72;

        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TreatmentHistoryService>? _logger;

        public TreatmentHistoryService(
            IDbContextFactory<ApplicationDbContext> dbFactory,
            IWebHostEnvironment env,
            ILogger<TreatmentHistoryService>? logger = null)
        {
            _dbFactory = dbFactory;
            _env = env;
            _logger = logger;
        }

        public static string BuildClientId(Appointment appointment)
        {
            if (!string.IsNullOrEmpty(appointment.UserId))
            {
                return appointment.UserId;
            }

            return $"guest:{appointment.Id}";
        }

        public static string SanitizeClientFolder(string clientId)
        {
            var s = clientId.Trim();
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                s = s.Replace(c, '_');
            }

            return s.Length > 120 ? s[..120] : s;
        }

        /// <summary>За рядком календаря (AppointmentService.Id) повертає ClientId для історії.</summary>
        public async Task<string?> GetClientIdFromAppointmentServiceAsync(int appointmentServiceId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var row = await db.AppointmentServices
                .AsNoTracking()
                .Include(x => x.Appointment)
                .FirstOrDefaultAsync(x => x.Id == appointmentServiceId, ct);
            if (row?.Appointment is null)
            {
                return null;
            }

            return BuildClientId(row.Appointment);
        }

        public async Task<Appointment?> GetLatestAppointmentForClientAsync(string clientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            if (clientId.StartsWith("guest:", StringComparison.OrdinalIgnoreCase))
            {
                if (clientId.Length > "guest:".Length && int.TryParse(clientId["guest:".Length..], out var apptId))
                {
                    return await db.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == apptId, ct);
                }

                return null;
            }

            return await db.Appointments
                .AsNoTracking()
                .Where(a => a.UserId == clientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.SetHour)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<(string? Email, string? Phone, string Name)?> GetClientContactAsync(string clientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            if (!clientId.StartsWith("guest:", StringComparison.OrdinalIgnoreCase))
            {
                var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == clientId, ct);
                if (user is null)
                {
                    return null;
                }

                var name = $"{user.FirstName} {user.LastName}".Trim();
                if (string.IsNullOrEmpty(name))
                {
                    name = user.UserName ?? user.Email ?? "Клієнт";
                }

                return (user.Email, user.PhoneNumber, name);
            }

            if (clientId.Length > "guest:".Length && int.TryParse(clientId["guest:".Length..], out var apptId))
            {
                var appt = await db.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == apptId, ct);
                if (appt is null)
                {
                    return null;
                }

                return (null, appt.ClientPhone, appt.ClientName);
            }

            return null;
        }

        public async Task<List<TreatmentHistory>> ListForClientAsync(string clientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.TreatmentHistories
                .AsNoTracking()
                .AsSplitQuery()
                .Include(h => h.Photos)
                .Where(h => h.ClientId == clientId)
                .OrderByDescending(h => h.VisitDate)
                .ThenByDescending(h => h.Id)
                .ToListAsync(ct);
        }

        public async Task<TreatmentHistory?> GetWithPhotosAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.TreatmentHistories
                .AsNoTracking()
                .AsSplitQuery()
                .Include(h => h.Photos)
                .FirstOrDefaultAsync(h => h.Id == id, ct);
        }

        /// <summary>Оновлює лише текстові/датові поля картки, не чіпаючи колекцію фото в БД.</summary>
        public async Task<bool> UpdateDetailsAsync(TreatmentHistory patch, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var e = await db.TreatmentHistories.FirstOrDefaultAsync(h => h.Id == patch.Id, ct);
            if (e is null)
            {
                return false;
            }

            e.VisitDate = patch.VisitDate;
            e.ProcedureName = patch.ProcedureName;
            e.ProcedureDescription = patch.ProcedureDescription;
            e.SkinBeforeDescription = patch.SkinBeforeDescription;
            e.SkinAfterDescription = patch.SkinAfterDescription;
            e.Recommendations = patch.Recommendations;
            e.MasterComment = patch.MasterComment;
            e.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<TreatmentHistory> CreateAsync(TreatmentHistory entity, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = null;
            db.TreatmentHistories.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(TreatmentHistory entity, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            entity.UpdatedAt = DateTime.UtcNow;
            db.TreatmentHistories.Update(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var entity = await db.TreatmentHistories.Include(h => h.Photos).FirstOrDefaultAsync(h => h.Id == id, ct);
            if (entity is null)
            {
                return false;
            }

            var root = GetPhysicalTreatmentRoot(entity.ClientId, entity.Id);
            if (Directory.Exists(root))
            {
                try
                {
                    Directory.Delete(root, recursive: true);
                }
                catch
                {
                    // ignore IO errors — БД все одно очистимо
                }
            }

            db.TreatmentHistories.Remove(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeletePhotoAsync(int photoId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var photo = await db.TreatmentPhotos
                .Include(p => p.TreatmentHistory)
                .FirstOrDefaultAsync(p => p.Id == photoId, ct);
            if (photo is null)
            {
                return false;
            }

            var physical = Path.Combine(_env.WebRootPath, photo.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(physical))
            {
                try
                {
                    File.Delete(physical);
                }
                catch
                {
                    // ignore
                }
            }

            db.TreatmentPhotos.Remove(photo);
            await db.SaveChangesAsync(ct);
            return true;
        }

        public string GetPhysicalTreatmentRoot(string clientId, int treatmentId) =>
            Path.Combine(_env.WebRootPath, "uploads", "treatments", SanitizeClientFolder(clientId), treatmentId.ToString());

        /// <summary>JPEG-прев’ю для відображення у формі після вибору файлу.</summary>
        public static string? TryGetPreviewDataUrlFromBytes(ReadOnlySpan<byte> imageBytes)
        {
            try
            {
                using var image = Image.Load(imageBytes);
                ResizeToMaxEdge(image, PreviewImageMaxEdge);
                using var ms = new MemoryStream();
                image.SaveAsJpeg(ms, new JpegEncoder { Quality = PreviewJpegQuality });
                return $"data:image/jpeg;base64,{Convert.ToBase64String(ms.ToArray())}";
            }
            catch
            {
                return null;
            }
        }

        private static void ResizeToMaxEdge(Image image, int maxEdge)
        {
            if (image.Width <= maxEdge && image.Height <= maxEdge)
            {
                return;
            }

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxEdge, maxEdge)
            }));
        }

        private static async Task<byte[]> ReadBrowserFileBytesAsync(IBrowserFile file, CancellationToken ct)
        {
            await using var s = file.OpenReadStream(maxAllowedSize: MaxTreatmentImageUploadBytes, cancellationToken: ct);
            using var ms = new MemoryStream();
            await s.CopyToAsync(ms, ct);
            return ms.ToArray();
        }

        /// <summary>Зберігає зображення як стиснутий JPEG (до @StoredImageMaxEdge px по довшій стороні). При збої декодування — копія сирих байтів (JPEG/PNG/WebP).</summary>
        private async Task<string> SaveCompressedJpegToDiskAsync(
            string clientId,
            int treatmentId,
            string beforeOrAfter,
            byte[] imageBytes,
            CancellationToken ct)
        {
            try
            {
                using var mem = new MemoryStream(imageBytes, writable: false);
                using var image = await Image.LoadAsync(mem, ct);
                ResizeToMaxEdge(image, StoredImageMaxEdge);

                var sub = beforeOrAfter.Equals(TreatmentPhoto.PhotoTypeAfter, StringComparison.OrdinalIgnoreCase)
                    ? TreatmentPhoto.PhotoTypeAfter
                    : TreatmentPhoto.PhotoTypeBefore;

                var dir = Path.Combine(GetPhysicalTreatmentRoot(clientId, treatmentId), sub);
                Directory.CreateDirectory(dir);

                var name = $"{Guid.NewGuid():N}.jpg";
                var fullPath = Path.Combine(dir, name);
                await image.SaveAsJpegAsync(fullPath, new JpegEncoder { Quality = StoredJpegQuality }, cancellationToken: ct);

                return $"/uploads/treatments/{SanitizeClientFolder(clientId)}/{treatmentId}/{sub}/{name}".Replace('\\', '/');
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Стиснення JPEG не вдалося, пробуємо зберегти оригінальні байти зображення.");
                try
                {
                    return await SaveRawImageBytesFallbackAsync(clientId, treatmentId, beforeOrAfter, imageBytes, ct);
                }
                catch (Exception inner)
                {
                    throw new InvalidOperationException(
                        "Не вдалося зберегти фото. Спробуйте інший формат (JPEG, PNG, WebP) або менший файл.",
                        inner);
                }
            }
        }

        private static string GuessImageFileExtension(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            {
                return ".jpg";
            }

            if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == (byte)'P' && bytes[2] == (byte)'N' && bytes[3] == (byte)'G')
            {
                return ".png";
            }

            if (bytes.Length >= 12
                && bytes[0] == (byte)'R'
                && bytes[1] == (byte)'I'
                && bytes[2] == (byte)'F'
                && bytes[3] == (byte)'F'
                && bytes[8] == (byte)'W'
                && bytes[9] == (byte)'E'
                && bytes[10] == (byte)'B'
                && bytes[11] == (byte)'P')
            {
                return ".webp";
            }

            return ".jpg";
        }

        private async Task<string> SaveRawImageBytesFallbackAsync(
            string clientId,
            int treatmentId,
            string beforeOrAfter,
            byte[] imageBytes,
            CancellationToken ct)
        {
            var ext = GuessImageFileExtension(imageBytes);
            var sub = beforeOrAfter.Equals(TreatmentPhoto.PhotoTypeAfter, StringComparison.OrdinalIgnoreCase)
                ? TreatmentPhoto.PhotoTypeAfter
                : TreatmentPhoto.PhotoTypeBefore;

            var dir = Path.Combine(GetPhysicalTreatmentRoot(clientId, treatmentId), sub);
            Directory.CreateDirectory(dir);

            var name = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(dir, name);
            await File.WriteAllBytesAsync(fullPath, imageBytes, ct);

            return $"/uploads/treatments/{SanitizeClientFolder(clientId)}/{treatmentId}/{sub}/{name}".Replace('\\', '/');
        }

        public async Task<TreatmentPhoto> AddPhotoFromBytesAsync(
            int treatmentHistoryId,
            string clientId,
            string beforeOrAfter,
            byte[] imageBytes,
            CancellationToken ct = default)
        {
            var path = await SaveCompressedJpegToDiskAsync(clientId, treatmentHistoryId, beforeOrAfter, imageBytes, ct);
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var type = beforeOrAfter.Equals(TreatmentPhoto.PhotoTypeAfter, StringComparison.OrdinalIgnoreCase)
                ? TreatmentPhoto.PhotoTypeAfter
                : TreatmentPhoto.PhotoTypeBefore;
            var photo = new TreatmentPhoto
            {
                TreatmentHistoryId = treatmentHistoryId,
                FilePath = path,
                PhotoType = type,
                UploadedAt = DateTime.UtcNow
            };
            db.TreatmentPhotos.Add(photo);
            await db.SaveChangesAsync(ct);
            return photo;
        }

        public async Task<TreatmentPhoto> AddPhotoAndPersistAsync(
            int treatmentHistoryId,
            string clientId,
            string beforeOrAfter,
            IBrowserFile file,
            CancellationToken ct = default)
        {
            var bytes = await ReadBrowserFileBytesAsync(file, ct);
            return await AddPhotoFromBytesAsync(treatmentHistoryId, clientId, beforeOrAfter, bytes, ct);
        }

        public async Task<string?> GetClientIdFromAppointmentIdAsync(int appointmentId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var appt = await db.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == appointmentId, ct);
            return appt is null ? null : BuildClientId(appt);
        }
    }
}
