using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

using webStudioBlazor.Data;

namespace webStudioBlazor.EntityModels
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введіть ім’я та прізвище")]
        [StringLength(100, ErrorMessage = "Ім'я не повинно перевищувати 100 символів")]
        [MinLength(3, ErrorMessage = "Ім'я занадто коротке")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер телефону")]
        [RegularExpression(@"^(\+380|0)\d{9}$", ErrorMessage = "Формат телефону: +380XXXXXXXXX")]
        public string ClientPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть дату запису")]
        public DateOnly AppointmentDate { get; set; }

        [Required(ErrorMessage = "Оберіть час запису")]
        public TimeOnly SetHour { get; set; }
               
        public int MasterId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Оберіть категорію")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Оберіть послугу")]
        public int TherapyId { get; set; }
             
        public decimal Price { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public Master? Master { get; set; }

        public Category? Category { get; set; }

        public TherapyCard? TherapyCard { get; set; }

        public List<AppointmentService?> AppointmentServices { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
