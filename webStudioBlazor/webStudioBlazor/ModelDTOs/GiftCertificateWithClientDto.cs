namespace webStudioBlazor.ModelDTOs
{
    public class GiftCertificateWithClientDto
    {
        public int Id { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
              
        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; }
    }
}
