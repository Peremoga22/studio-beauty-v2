using Microsoft.AspNetCore.Identity;

using webStudioBlazor.Data;

namespace webStudioBlazor.EntityModels
{
    public class Appointment
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly SetHour { get; set; }
        public int MasterId { get; set; }
        public int CategoryId { get; set; }
        public int TherapyId { get; set; }
        public decimal Price { get; set; }
        public bool IsCompleted { get; set; }
        public string? UserId { get; set; } = default!;
        public ApplicationUser? User { get; set; } = default!;
        public Master Master { get; set; } = default!;
        public Category Category { get; set; } = default!;
        public TherapyCard TherapyCard { get; set; } = default!;
        public List<AppointmentService?> AppointmentServices { get; set; } = new();
       
    }
}
