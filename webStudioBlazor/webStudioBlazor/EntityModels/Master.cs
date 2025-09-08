namespace webStudioBlazor.EntityModels
{
    public class Master
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public List<Category?> Categories { get; set; } = new();
    }
}
