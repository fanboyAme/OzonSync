using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public long OzonOrderId { get; set; }
        public OrderStatus Status { get; set; } //public enum OrderStatus { New, Processing, Delivered, Cancelled, Returned}
        public DateTime CreatedAt { get; set; }
        public DateTime InProccesAt { get; set; }

        public Order(Guid shopId, long ozonOrderId, OrderStatus status, DateTime createdAt, DateTime inProccesAt)
        {
            Id = Guid.NewGuid();
            OzonOrderId = ozonOrderId;
            Status = status;
            CreatedAt = createdAt;
            InProccesAt = inProccesAt;
        }

    }
}
