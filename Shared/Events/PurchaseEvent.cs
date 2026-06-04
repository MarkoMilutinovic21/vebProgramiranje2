using System.Runtime.Serialization;

namespace Shared.Events
{
    [DataContract]
    public class PurchaseEvent
    {
        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string BookId { get; set; }

        [DataMember]
        public string BookTitle { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal TotalPrice { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}