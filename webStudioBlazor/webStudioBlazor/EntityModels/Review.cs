using System.ComponentModel.DataAnnotations;

using webStudioBlazor.Data;

namespace webStudioBlazor.EntityModels
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5, ErrorMessage = "Оцінка має бути від 1 до 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Текст відгуку не може бути порожнім")]
        [StringLength(1000, ErrorMessage = "Максимальна довжина відгуку – 1000 символів")]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
             
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = default!;

        [Required(ErrorMessage = "Категорія не вибрана")]
        [Range(1, int.MaxValue, ErrorMessage = "Будь ласка, оберіть категорію")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } =  default!;

        [Required(ErrorMessage = "Послуга не вибрана")]
        [Range(1, int.MaxValue, ErrorMessage = "Будь ласка, оберіть послугу")]
        public int TherapyId { get; set; }
        public TherapyCard TherapyCard { get; set; } = default!;

        [Required(ErrorMessage = "Майстер не вибраний")]
        [Range(1, int.MaxValue, ErrorMessage = "Будь ласка, оберіть майстра")]
        public int MasterId { get; set; }
        public Master Master { get; set; } = default!;

      
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
