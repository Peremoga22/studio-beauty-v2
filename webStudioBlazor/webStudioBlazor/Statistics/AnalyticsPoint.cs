namespace webStudioBlazor.Statistics
{
    public class AnalyticsPoint
    {
        public DateOnly Date { get; set; }
        public int CosmetologyCount { get; set; }
        public int MassageCount { get; set; }
        public decimal CosmetologyRevenue { get; set; }
        public decimal MassageRevenue { get; set; }

        public decimal TotalRevenue => CosmetologyRevenue + MassageRevenue;
       
    }
}
