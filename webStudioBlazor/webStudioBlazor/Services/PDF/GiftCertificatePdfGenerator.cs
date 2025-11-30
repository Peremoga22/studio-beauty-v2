using QuestPDF.Drawing; // Add this using directive for ImageSource
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using webStudioBlazor.EntityModels;

namespace webStudioBlazor.Services.PDF
{
    public class GiftCertificatePdfGenerator
    {

        public byte[] Generate(
         string logoPath,
         string brandName,
         string certificateTitle,
         string recipientName,
         decimal amount,
         string currencySymbol,
         string personalMessage,
         string fromName,
         string certificateNumber,
         DateTime issueDate,
         string validityText)
        {
            byte[]? logoImage = null;

            if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                logoImage = System.IO.File.ReadAllBytes(logoPath);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(20);
                    page.PageColor("#fdf4e3"); // світлий теплий фон

                    page.Content().AlignCenter().AlignMiddle().Element(content =>
                    {
                        content
                            .Width(PageSizes.A5.Landscape().Width - 60) // щоб точно влізло
                            .Height(PageSizes.A5.Landscape().Height - 60)
                            .Background("#ffffff")
                            .Border(1)
                            .BorderColor("#f0d3a6")
                            .Padding(20)
                            .Column(col =>
                            {
                                col.Spacing(12);

                                // ==== Верх: лого + назва ====
                                col.Item().Row(row =>
                                {
                                    row.Spacing(10);

                                    if (logoImage != null)
                                    {
                                        row.ConstantItem(50)
                                           .Height(50)
                                           .Image(logoImage)
                                           .FitArea();
                                    }

                                    row.RelativeItem().Column(h =>
                                    {
                                        h.Item().Text(brandName ?? "Назва студії")
                                            .FontSize(12)
                                            .FontColor("#7b4b2a")
                                            .SemiBold();

                                        h.Item().Text(certificateTitle ?? "Подарунковий сертифікат")
                                            .FontSize(18)
                                            .Bold()
                                            .FontColor("#4b2e16");
                                    });
                                });

                                // Тонка лінія
                                col.Item().LineHorizontal(1)
                                    .LineColor("#f0d3a6");

                                // ==== Номінал ====
                                col.Item().AlignCenter().Text($"{amount:N0} {currencySymbol}")
                                    .FontSize(28)
                                    .Bold()
                                    .FontColor("#c51d34");

                                // ==== Отримувач ====
                                col.Item().Column(info =>
                                {
                                    info.Spacing(3);

                                    info.Item().AlignCenter().Text("Отримувач")
                                        .FontSize(10)
                                        .FontColor("#777")
                                        .SemiBold();

                                    info.Item().AlignCenter().Text(
                                            string.IsNullOrWhiteSpace(recipientName)
                                                ? "Ім’я отримувача"
                                                : recipientName)
                                        .FontSize(18)
                                        .Bold()
                                        .FontColor("#333");
                                });

                                // ==== Персональне побажання (якщо є) ====
                                if (!string.IsNullOrWhiteSpace(personalMessage))
                                {
                                    col.Item().AlignCenter().Text(personalMessage)
                                        .FontSize(11)
                                        .Italic()
                                        .FontColor("#555");
                                }

                                // Тонка лінія
                                col.Item().LineHorizontal(1)
                                    .LineColor("#f0d3a6");

                                // ==== Нижній блок: №, дата, термін, від кого ====
                                col.Item().Row(row =>
                                {
                                    row.Spacing(15);

                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("Номер сертифіката")
                                            .FontSize(8)
                                            .FontColor("#777")
                                            .SemiBold();
                                        c.Item().Text(string.IsNullOrWhiteSpace(certificateNumber) ? "—" : certificateNumber)
                                            .FontSize(10)
                                            .FontColor("#333");
                                    });

                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("Дата оформлення")
                                            .FontSize(8)
                                            .FontColor("#777")
                                            .SemiBold();
                                        c.Item().Text(issueDate.ToString("dd.MM.yyyy"))
                                            .FontSize(10)
                                            .FontColor("#333");
                                    });

                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("Термін дії")
                                            .FontSize(8)
                                            .FontColor("#777")
                                            .SemiBold();
                                        c.Item().Text(string.IsNullOrWhiteSpace(validityText) ? "—" : validityText)
                                            .FontSize(10)
                                            .FontColor("#333");
                                    });

                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("Від кого")
                                            .FontSize(8)
                                            .FontColor("#777")
                                            .SemiBold();
                                        c.Item().Text(string.IsNullOrWhiteSpace(fromName) ? "Лесі" : fromName)
                                            .FontSize(10)
                                            .FontColor("#333");
                                    });
                                });

                                // Маленький футер усередині того ж листка
                                col.Item().AlignCenter().Text(
                                        "Сертифікат дійсний за попереднім записом. " +
                                        "Пред’явіть його адміністратору перед початком процедури.")
                                    .FontSize(8)
                                    .FontColor("#9a7b55");
                            });
                    });
                });
            });

            return document.GeneratePdf();
        }

    }
}
