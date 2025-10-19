namespace webStudioBlazor.EntityModels
{
    public class Order
    {
        public int Id { get; set; }              
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string OrderStatus { get; set; } = "New";
    }
}
