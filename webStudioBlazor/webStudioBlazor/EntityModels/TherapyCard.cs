using System.ComponentModel.DataAnnotations;

namespace webStudioBlazor.EntityModels
{
    public class TherapyCard
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Вкажіть назву картки.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Довжина назви 3–60 символів.")]
        [RegularExpression(@"^[\p{L}\p{M}\d\s\-\’'.,!&()]+$",
        ErrorMessage = "Назва може містити літери, цифри, пробіли та базові знаки пунктуації.")]
        public string TitleCard { get; set; }

        [Required(ErrorMessage = "Додайте короткий опис.")]        
        public string DescriptionCard { get; set; }

        public string ImagePath { get; set; }

        [Range(typeof(decimal), "1", "1000000", ErrorMessage = "Ціна має бути більшою за 0.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Оберіть категорію.")]
        public int CategoryId { get; set; }
        public Category Categories { get; set; } = default!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Якщо створення нової картки — фото обов’язкове
            if (Id == 0 && string.IsNullOrWhiteSpace(ImagePath))
                yield return new ValidationResult("Додайте фото послуги.", new[] { nameof(ImagePath) });
        }
    }
}
