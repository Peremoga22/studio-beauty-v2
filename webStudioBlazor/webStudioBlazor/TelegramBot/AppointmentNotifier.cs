﻿using Microsoft.Extensions.Options;

using System.Text;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using webStudioBlazor.EntityModels;

using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task NotifyNewOrderAsync(Order order,
                                      long? overrideChatId = null,
                                      CancellationToken ct = default)
        {
            var chatId = overrideChatId ?? _cfg.AdminChatId;

            // Клієнт (null-safe)
            var co = order?.ClientOrder;
            var first = co?.ClientFirstName ?? string.Empty;
            var last = co?.ClientLastName ?? string.Empty;
            var phone = co?.ClientPhone ?? "—";
            var city = co?.City ?? "—";
            var npo = co?.AddressNewPostOffice ?? "—";
            var date = order?.OrderDate.Date ?? DateTime.UtcNow.Date;

            // Позиції (null-safe; Quantity вважаємо int; якщо int? — заміни рядок з qty на (i.Quantity ?? 1))
            var itemsText = new StringBuilder();
            foreach (var i in (order?.Items ?? Enumerable.Empty<OrderItem>()))
            {
                var title = i.Therapy?.TitleCard ?? "Без назви";
                var qty = Math.Max(1, i.Quantity); // якщо Quantity int? => var qty = i.Quantity ?? 1;
                var price = i.UnitPrice;
                var line = price * qty;
                itemsText.AppendLine($"• *{Esc(title)}* — {qty} × {price:0}₴ = *{line:0}₴*");
            }

            // Разом (fallback на TotalAmount, якщо Items порожні або нема)
            decimal total = 0m;
            if (order?.Items is { Count: > 0 })
                total = order.Items.Sum(i => i.UnitPrice * Math.Max(1, i.Quantity)); // якщо int? => (i.Quantity ?? 1)
            else
                total = order?.TotalAmount ?? 0m;

            var text = new StringBuilder()
                .AppendLine("🛍️ *Нове замовлення!*")
                .AppendLine($"🆔 Замовлення № *{order?.Id}*")
                .AppendLine($"📅 Дата: *{date:dd.MM.yyyy}*")
                .AppendLine($"👤 Клієнт: *{Esc(first)} {Esc(last)}*")
                .AppendLine($"📞 Телефон: `{Esc(phone)}`")
                .AppendLine($"🏙️ Місто: *{Esc(city)}*")
                .AppendLine($"📦 Відділення НП: *{Esc(npo)}*")
                .AppendLine()
                .AppendLine("*Товари:*")
                .Append(itemsText) // не null
                .AppendLine()
                .AppendLine($"💰 *Разом:* {total:0}₴")
                .AppendLine("💳 Оплата: *Післяплата при отриманні*")
                .AppendLine("🚚 Доставка: *Нова Пошта*")
                .ToString();

            await _bot.SendMessage(chatId, text, parseMode: ParseMode.Markdown, cancellationToken: ct);
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
