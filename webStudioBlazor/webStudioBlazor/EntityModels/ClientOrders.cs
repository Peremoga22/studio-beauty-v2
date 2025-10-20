using System.ComponentModel.DataAnnotations;

namespace webStudioBlazor.EntityModels
{
    public class ClientOrders
    {
        public int Id { get; set; }

        [Required, MaxLength(64)]
        public string ClientFirstName { get; set; }

        [Required, MaxLength(64)]
        public string ClientLastName { get; set; }

        public string ClientPhone { get; set; }
        public DateOnly AppointmentDate { get; set; }

        [Required, MaxLength(64)]
        public string City { get; set; }

        [Required, MaxLength(128)]
        public string AddressNewPostOffice { get; set; }

        [Range(0, 1_000_000)]
        public decimal Price { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;
    }
}
