using System.ComponentModel.DataAnnotations.Schema;

namespace webStudioBlazor.EntityModels
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;
        public int TherapyId { get; set; }
        public TherapyCard Therapy { get; set; } = default!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
