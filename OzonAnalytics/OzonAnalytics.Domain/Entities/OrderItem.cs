namespace OzonAnalytics.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal CommisionAmount { get; set; }
        public decimal? LogisticsAmount { get; set; }
        public decimal Payout { get; set; }
        public OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, int quantity, decimal price, decimal commisionAmount, decimal? logisticsAmount, decimal payout)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            CommisionAmount = commisionAmount;
            LogisticsAmount = logisticsAmount;
            Payout = payout;
        }
    }
}
