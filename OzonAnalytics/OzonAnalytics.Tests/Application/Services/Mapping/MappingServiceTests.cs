using OzonAnalytics.Application.Services.Mapping;

using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Tests.Application.Services.Mapping
{
    public class MappingServiceTests
    {
        [Theory]
        [InlineData("delivering", "posting_received", OrderStatusAndSubStatus.Delivered)]
        [InlineData("awaiting_deliver", "posting_transferring_to_delivery", OrderStatusAndSubStatus.HandedOver)]
        [InlineData("awaiting_registration", "posting_transferring_to_delivery", OrderStatusAndSubStatus.Assembling)]
        [InlineData("delivering", "posting_on_way_to_pickup_point", OrderStatusAndSubStatus.InTransit)]
        [InlineData("awaiting_packaging", "posting_in_client_arbitration", OrderStatusAndSubStatus.Problem)]
        [InlineData("arbitration", "", OrderStatusAndSubStatus.Problem)]
        [InlineData("cancelled", "", OrderStatusAndSubStatus.Cancelled)]
        [InlineData("abc", "", OrderStatusAndSubStatus.Unknown)]
        [InlineData(null, null, OrderStatusAndSubStatus.Unknown)]
        [InlineData("AWAITING_PACKAGING", " ", OrderStatusAndSubStatus.Assembling)]
        public void MapStatusesTest(string? status, string? substatus, OrderStatusAndSubStatus excepted)
        {
            var result = MappingService.Map(status, substatus);

            Assert.Equal(excepted, result);
        }
    }
}
