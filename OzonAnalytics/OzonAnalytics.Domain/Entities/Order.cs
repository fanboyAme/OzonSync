using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Domain.Entities
{
    internal class Orders
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public long OzonOrderId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime InProccesAt { get; set; }

        public Orders(Guid shopId, long ozonOrderId, OrderStatus status, DateTime createdAt, DateTime inProccesAt)
        {
            Id = Guid.NewGuid();
            OzonOrderId = ozonOrderId;
            Status = status;
            CreatedAt = createdAt;
            InProccesAt = inProccesAt;
        }

    }
}
