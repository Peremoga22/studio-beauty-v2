namespace webStudioBlazor.ModelDTOs
{
    public class AppointmentWithDetailsDto
    {
        public int Id { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public int MasterId { get; set; }
        public int CategoryId { get; set; }
        public int TherapyId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
        public string MasterFullName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsVideo { get; set; }
    }
}
