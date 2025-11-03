namespace webStudioBlazor.ModelDTOs
{
    public sealed class OrderRow
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string NewPostOffice { get; set; } = string.Empty;

        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }          
        public decimal UnitPrice { get; set; }     
        public decimal LineTotal { get; set; }     

        public int MoreItemsCount { get; set; }    
        public decimal OrderTotal { get; set; }  
        
        public bool IsShownInOrder { get; set; }
    }
}
