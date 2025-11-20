using System.ComponentModel.DataAnnotations;

using webStudioBlazor.Data;

namespace webStudioBlazor.EntityModels
{
    public class ClientOrders
    {
        public int Id { get; set; }      

        [Required(ErrorMessage = "Вкажіть ім’я")]
        [StringLength(64, ErrorMessage = "Ім’я має містити не більше 64 символів")]
        public string ClientFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть прізвище")]
        [StringLength(64, ErrorMessage = "Прізвище має містити не більше 64 символів")]
        public string ClientLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть номер телефону")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Телефон має бути у форматі +380XXXXXXXXX")]
        public string ClientPhone { get; set; } = string.Empty;
        public DateOnly AppointmentDate { get; set; }

        [Required(ErrorMessage = "Вкажіть місто")]
        [StringLength(64, ErrorMessage = "Місто має містити не більше 64 символів")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть відділення Нової Пошти")]
        [StringLength(128, ErrorMessage = "Назва відділення задовга (до 128 символів)")]
        public string AddressNewPostOffice { get; set; } = string.Empty;

        [Range(0, 1_000_000)]
        public decimal Price { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;
        public string? UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; } = default!;
    }
}
