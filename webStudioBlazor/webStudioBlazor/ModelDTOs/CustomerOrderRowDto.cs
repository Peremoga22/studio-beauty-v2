namespace webStudioBlazor.ModelDTOs
{
    public sealed class CustomerOrderRowDto
    {        
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
               
        public string ClientFullName { get; set; } = "";
        public string ClientPhone { get; set; } = "";
        public string City { get; set; } = "";
        public string NewPostOffice { get; set; } = "";       
        public string? AddressNewPostOffice { get; set; }     
              
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

}
