namespace webStudioBlazor.EntityModels
{
    public class AppointmentService
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = default!;

        public int  CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public int TherapyId { get; set; }
        public TherapyCard TherapyCard { get; set; } = default!;
      
    }
}
