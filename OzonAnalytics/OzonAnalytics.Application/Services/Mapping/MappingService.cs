using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Application.Services.Mapping
{
    public static class MappingService
    {
        private static readonly Dictionary<string, OrderStatusAndSubStatus> Status = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, OrderStatusAndSubStatus> Substatus = new(StringComparer.OrdinalIgnoreCase);

        static MappingService()
        {
            Add(Status, new Dictionary<OrderStatusAndSubStatus, string[]>
            {
                [OrderStatusAndSubStatus.Assembling] = ["awaiting_verification", "awaiting_approve", "awaiting_packaging", "awaiting_registration"],
                [OrderStatusAndSubStatus.HandedOver] = ["awaiting_deliver", "acceptance_in_progress"],
                [OrderStatusAndSubStatus.InTransit] = ["delivering", "driver_pickup"],
                [OrderStatusAndSubStatus.Problem] = ["arbitration", "client_arbitration", "not_accepted"],
                [OrderStatusAndSubStatus.Cancelled] = ["cancelled", "cancelled_from_split_pending"],
            });

            Add(Substatus, new Dictionary<OrderStatusAndSubStatus, string[]>
            {
                [OrderStatusAndSubStatus.Assembling] = ["posting_created", "posting_split_pending", "posting_awaiting_registration", "posting_registered", "posting_awaiting_passport_data"],
                [OrderStatusAndSubStatus.HandedOver] = ["posting_acceptance_in_progress", "posting_transferred_to_courier_service", "posting_in_carriage"],
                [OrderStatusAndSubStatus.InTransit] = ["posting_driver_pick_up", "posting_on_way_to_city", "posting_on_way_to_pickup_point", "posting_in_courier_service", "posting_in_pickup_point"],
                [OrderStatusAndSubStatus.Delivered] = ["posting_delivered", "posting_received", "posting_conditionally_delivered"],
                [OrderStatusAndSubStatus.Problem] = ["posting_in_arbitration", "posting_in_client_arbitration", "posting_registration_error", "posting_not_in_carriage", "posting_not_in_sort_center", "ship_failed"],
                [OrderStatusAndSubStatus.Cancelled] = ["posting_canceled", "posting_returned_to_warehouse"],
            });

        }

        private static void Add(Dictionary<string, OrderStatusAndSubStatus> mainDictionary, Dictionary<OrderStatusAndSubStatus, string[]> newDictinary)
        {

            foreach (var (key, value) in newDictinary)
            {
                foreach (var item in value)
                {
                    mainDictionary.Add(item, key);
                }
            }

        }

        public static OrderStatusAndSubStatus GetStatus(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && Status.TryGetValue(value.Trim(), out var res)) return res;
            return OrderStatusAndSubStatus.Unknown;
        }
        public static OrderStatusAndSubStatus GetSubstatus(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && Substatus.TryGetValue(value.Trim(), out var res)) return res;
            return OrderStatusAndSubStatus.Unknown;
        }

        public static OrderStatusAndSubStatus Map(string? status, string? substatus)
        {
            var bySubstatus = GetSubstatus(substatus);
            if (bySubstatus != OrderStatusAndSubStatus.Unknown) return bySubstatus;
            return GetStatus(status);
        }
    }
}
