namespace webStudioBlazor.ModelDTOs
{
    public class ClientOrderWithDetailsDto
    {
        public int Id { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public List<ClientOrderItemDto> Items { get; set; } = new();
    }
}
