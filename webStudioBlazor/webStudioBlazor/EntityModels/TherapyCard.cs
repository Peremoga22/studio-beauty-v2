namespace webStudioBlazor.EntityModels
{
    public class TherapyCard
    {
        public int Id { get; set; }
        public string TitleCard { get; set; }
        public string DescriptionCard { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Categories { get; set; } = default!;
    }
}
