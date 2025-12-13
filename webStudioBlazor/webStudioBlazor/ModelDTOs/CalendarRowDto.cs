namespace webStudioBlazor.ModelDTOs
{
    public class CalendarRowDto
    {
        public int Id { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;

        public string ServiceName { get; set; } = string.Empty;   
        public string CategoryName { get; set; } = string.Empty;  
        public string MasterName { get; set; } = string.Empty; 
        public decimal? Price { get; set; }
        public bool IsCompleted { get; set; }
    }
}
