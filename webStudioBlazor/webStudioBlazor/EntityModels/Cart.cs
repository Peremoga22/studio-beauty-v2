namespace webStudioBlazor.EntityModels
{
    public class Cart
    {
        public int Id { get; set; }               
        public string SessionKey { get; set; } = default!;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
