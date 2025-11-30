using QuestPDF.Fluent;
using QuestPDF.Helpers;

using webStudioBlazor.EntityModels;

namespace webStudioBlazor.Services.PDF
{
    public class GiftCertificatePdfGenerator
    {
        public byte[] Generate(GiftCertificate cert)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(20);
                    page.Background("#ffffff");

                    page.Content().Padding(20).Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text("Shine Cosmetology")
                            .FontSize(18).Bold().FontColor("#e26da6");

                        col.Item().Text("ПОДАРУНКОВИЙ СЕРТИФІКАТ")
                            .FontSize(26).Bold().FontColor("#333");

                        col.Item().LineHorizontal(1).LineColor("#f8b7d8");

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Отримувач").Bold();
                                c.Item().Text(cert.RecipientName).FontSize(16);

                                if (!string.IsNullOrWhiteSpace(cert.Message))
                                {
                                    c.Item().Text($"💌 {cert.Message}")
                                        .FontSize(12)
                                        .Italic()
                                        .FontColor("#666");
                                }
                            });

                            row.ConstantItem(200).Column(c =>
                            {
                                c.Item().Text("Номінал").Bold();
                                c.Item().Text($"{cert.Amount:N0} грн")
                                    .FontSize(20).Bold().FontColor("#e26da6");
                            });
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Сертифікат №").Bold();
                                c.Item().Text($"#{cert.Id}");
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Створено").Bold();
                                c.Item().Text(cert.CreatedAt.ToString("dd.MM.yyyy"));
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Термін дії").Bold();
                                c.Item().Text("3 місяці");
                            });
                        });

                        col.Item().AlignCenter().Text("Студія краси «Shine Cosmetology»")
                            .FontSize(12).FontColor("#999");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
