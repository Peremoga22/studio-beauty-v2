using System.ComponentModel.DataAnnotations;

using webStudioBlazor.Data;

namespace webStudioBlazor.EntityModels
{
    public class GiftCertificate
    {
        public int Id { get; set; }
              
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Вкажіть ім'я отримувача")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть суму сертифіката")]
        [Range(100, 100000, ErrorMessage = "Сума повинна бути від 100 до 100 000 грн")]
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(500, ErrorMessage = "Повідомлення не повинно перевищувати 500 символів")]
        public string? Message { get; set; }                      
        public bool IsApproved { get; set; } = false;
    }
}
