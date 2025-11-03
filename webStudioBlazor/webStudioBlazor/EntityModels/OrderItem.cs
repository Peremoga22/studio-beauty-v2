using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webStudioBlazor.EntityModels
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;
        public int? TherapyId { get; set; }
        public TherapyCard? Therapy { get; set; } = default!;
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        [Range(1, 1000)]
        public int Quantity { get; set; } = 1;
        [NotMapped]
        public decimal TotalPrice => UnitPrice * Quantity;

        public string? SessionKey { get; set; }
        public bool IsShownInOrder { get; set; }
    }
}
