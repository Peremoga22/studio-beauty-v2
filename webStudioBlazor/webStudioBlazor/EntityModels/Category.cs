namespace webStudioBlazor.EntityModels
{
    public class Category
    {
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public int MasterId { get; set; }
        public List<PageTherapy?> PageTherapy { get; set; } = new();
        public List<TherapyCard?> TherapyCards { get; set; } = new();
        public Master Masters { get; set; } = default!;
    }
}
