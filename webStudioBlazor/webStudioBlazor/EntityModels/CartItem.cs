using System.ComponentModel.DataAnnotations.Schema;

namespace webStudioBlazor.EntityModels
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } = default!;
        public int TherapyId { get; set; }
        public TherapyCard Therapy { get; set; } = default!;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
