namespace webStudioBlazor.BL
{
    public sealed class TelegramOptions
    {
        public string BotToken { get; set; } = default!;
        public string BotUsername { get; set; } = default!;
        public string WebhookSecret { get; set; } = default!;
        public string BaseDeepLink { get; set; } = "https://t.me";
        public long? AdminChatId { get; set; }
    }
}
