namespace webStudioBlazor.EntityModels
{
    public class PageTherapy
    {
        public int Id { get; set; }
        public string TitlePage { get; set; }
        public string DescriptionPage { get; set; }       
        public int CategoryId { get; set; }
        public Category Categories { get; set; } = default!;
    }
}
