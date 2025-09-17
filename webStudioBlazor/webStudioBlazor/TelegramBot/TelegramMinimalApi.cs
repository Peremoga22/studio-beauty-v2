using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using webStudioBlazor.BL;
using webStudioBlazor.Data;
using webStudioBlazor.EntityModels;

public static class TelegramMinimalApi
{
    //public static void MapTelegramEndpoints(this WebApplication app)
    //{
    //    app.MapPost("/tg-webhook", async (
    //        HttpRequest req,
    //        [FromBody] Update update,
    //        ITelegramBotClient bot,
    //        IOptions<TelegramOptions> tgOpts,
    //        ApplicationDbContext db,
    //        ILoggerFactory lf) =>
    //    {
    //        var log = lf.CreateLogger("TG");
    //        var cfg = tgOpts.Value;

    //        // 1) Перевірка секрету
    //        if (!req.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var got) || got != cfg.WebhookSecret)
    //        {
    //            log.LogWarning("Unauthorized webhook: secret mismatch. Got: '{Got}'", got.ToString());
    //            return Results.Unauthorized();
    //        }

    //        // 2) Діагностика: що прийшло
    //        log.LogInformation("Update {Id} type {Type}", update.Id, update.Type);

    //        var msg = update.Message ?? update.EditedMessage;
    //        if (msg?.Text is string text)
    //        {
    //            log.LogInformation("Text from {ChatId}: {Text}", msg.Chat.Id, text);

    //            // 3) Парсинг deep-link: /start <payload>
    //            if (text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
    //            {
    //                var payload = ExtractStartPayload(text, cfg.BotUsername);

    //                if (!string.IsNullOrWhiteSpace(payload) && int.TryParse(payload, out var clientId))
    //                {
    //                    var client = await db.Appointments.FindAsync(clientId);
    //                    if (client is not null)
    //                    {
    //                        var ChatId = msg.Chat.Id;
    //                        var TelegramSubscribedAt = DateTime.UtcNow;
    //                        await db.SaveChangesAsync();

    //                        await bot.SendMessage(
    //                            chatId: msg.Chat.Id,
    //                            text: $"Вітаю, {client.ClientName}! Підписка на нагадування активована ✅");

    //                        log.LogInformation("Saved chatId {ChatId} for client {ClientId}", msg.Chat.Id, clientId);
    //                    }
    //                    else
    //                    {
    //                        await bot.SendMessage(msg.Chat.Id, "Не знайшов ваш запис у системі ❌");
    //                        log.LogWarning("ClientId {ClientId} not found", clientId);
    //                    }
    //                }
    //                else
    //                {
    //                    await bot.SendMessage(msg.Chat.Id, "Вкажіть ваш ID після /start, напр.: /start 123");
    //                    log.LogWarning("No or invalid payload after /start. Raw: '{Text}'", text);
    //                }
    //            }
    //        }

    //        return Results.Ok();
    //    });
    //}

    //private static string ExtractStartPayload(string text, string? botUsername)
    //{
    //    // Варіанти: "/start 123", "/start@shine_cosmo_bot 123"
    //    var t = text.Trim();

    //    // прибрати "/start" або "/start@bot"
    //    if (t.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
    //    {
    //        t = t[6..].Trim(); // після "/start"
    //        if (!string.IsNullOrEmpty(botUsername))
    //        {
    //            var atUser = "@" + botUsername.TrimStart('@');
    //            if (t.StartsWith(atUser, StringComparison.OrdinalIgnoreCase))
    //            {
    //                // випадок "/start@bot 123"
    //                t = t[atUser.Length..].Trim();
    //            }
    //        }
    //    }

    //    // тепер t — це payload (може бути "123")
    //    return t;
    //}
}
