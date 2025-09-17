using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace webStudioBlazor.BL
{
    public class AppointmentNotifier
    {
        private readonly ITelegramBotClient _bot;
        private readonly TelegramOptions _cfg;
        public AppointmentNotifier(ITelegramBotClient bot, IOptions<TelegramOptions> cfg)
        {
            _bot = bot;
            _cfg = cfg.Value;
        }

        public async Task NotifyNewAsync(string clientName, string phone, string serviceName,
                                         DateOnly date, TimeOnly time, string? notes = null,
                                         long? overrideChatId = null, CancellationToken ct = default)
        {
            var chatId = overrideChatId ?? _cfg.AdminChatId;

            var text = new StringBuilder()
                .AppendLine("🗓️ *Новий запис!*")
                .AppendLine($"👤 Клієнт: *{Esc(clientName)}*")
                .AppendLine($"📞 Телефон: `{Esc(phone)}`")
                .AppendLine($"💆 Послуга: *{Esc(serviceName)}*")
                .AppendLine($"📅 Дата: *{date:dd.MM.yyyy}*")
                .AppendLine($"⏰ Час: *{time:HH\\:mm}*")
                .AppendLine(!string.IsNullOrWhiteSpace(notes) ? $"📝 Примітки: {Esc(notes!)}" : "")
                .ToString();

            await _bot.SendMessage(chatId, text, parseMode: ParseMode.Markdown, cancellationToken: ct);
        }

        private static string Esc(string s) => s
            .Replace("_", "\\_").Replace("*", "\\*").Replace("[", "\\[").Replace("`", "\\`");
    }
}
