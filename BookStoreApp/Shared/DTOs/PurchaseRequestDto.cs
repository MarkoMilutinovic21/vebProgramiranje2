namespace Shared.DTOs
{
    public class PurchaseRequestDto
    {
        public string UserId { get; set; }
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public string Email { get; set; }
    }
}