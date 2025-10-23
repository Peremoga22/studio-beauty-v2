namespace webStudioBlazor.Statistics
{
    public class AnalyticsPoint
    {
        public DateOnly Date { get; set; }
        public int CosmetologyCount { get; set; }
        public int MassageCount { get; set; }
        public decimal CosmetologyRevenue { get; set; }
        public decimal MassageRevenue { get; set; }
        public int OrdersCount { get; set; }          // кількість замовлень у цей день
        public decimal SalesRevenue { get; set; }

        public int TotalCount => CosmetologyCount + MassageCount + OrdersCount;
        public decimal TotalRevenue => CosmetologyRevenue + MassageRevenue + SalesRevenue;

    }
}
