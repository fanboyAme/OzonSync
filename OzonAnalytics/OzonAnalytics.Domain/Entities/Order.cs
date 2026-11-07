namespace OzonAnalytics.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public string OzonOrderId { get; set; }
        public string Status { get; set; } = default!;
        public string Substatus { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime InProcessAt { get; set; }
        public Order() { }

        public Order(Guid shopId, string ozonOrderId, string status, string substatus, DateTime createdAt, DateTime inProccesAt)
        {
            Id = Guid.NewGuid();
            ShopId = shopId;
            OzonOrderId = ozonOrderId;
            Status = status;
            Substatus = substatus;
            CreatedAt = createdAt;
            InProcessAt = inProccesAt;
        }

    }
}
