namespace webStudioBlazor.ModelDTOs
{
    public sealed class CustomerSummaryDto
    {
        public int? ClientId { get; set; }        
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string City { get; set; } = "";
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateOnly? LastOrderDate { get; set; }
    }
}
