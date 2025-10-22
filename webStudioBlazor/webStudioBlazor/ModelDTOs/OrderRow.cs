namespace webStudioBlazor.ModelDTOs
{
    public sealed class OrderRow
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string NewPostOffice { get; set; } = string.Empty;

        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }          // 🔸 кількість
        public decimal UnitPrice { get; set; }     // 🔸 ціна за одиницю
        public decimal LineTotal { get; set; }     // 🔸 UnitPrice * Quantity

        public int MoreItemsCount { get; set; }    // інші позиції в цьому ж замовленні
        public decimal OrderTotal { get; set; }    // 🔸 сума всього замовлення
    }
}
