namespace webStudioBlazor.ModelDTOs
{
    public class ClientOrderItemDto
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
