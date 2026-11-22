using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzonAnalytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Orders\" SET\r\n \"Status\" = CASE\r\n  WHEN \"Substatus\" IN ('posting_delivered','posting_received','posting_conditionally_delivered') THEN 'Delivered'\r\n  WHEN \"Substatus\" IN ('posting_created','posting_split_pending','posting_awaiting_registration','posting_registered','posting_awaiting_passport_data') THEN 'Assembling'\r\n  WHEN \"Substatus\" IN ('posting_acceptance_in_progress','posting_transferred_to_courier_service','posting_in_carriage') THEN 'HandedOver'\r\n  WHEN \"Substatus\" IN ('posting_driver_pick_up','posting_on_way_to_city','posting_on_way_to_pickup_point','posting_in_courier_service','posting_in_pickup_point') THEN 'InTransit'\r\n  WHEN \"Substatus\" IN ('posting_in_arbitration','posting_in_client_arbitration','posting_registration_error','posting_not_in_carriage','posting_not_in_sort_center','ship_failed') THEN 'Problem'\r\n  WHEN \"Substatus\" IN ('posting_canceled','posting_returned_to_warehouse') THEN 'Cancelled'\r\n  WHEN \"Status\" = 'delivered' THEN 'Delivered'\r\n  WHEN \"Status\" IN ('awaiting_verification','awaiting_approve','awaiting_packaging','awaiting_registration') THEN 'Assembling'\r\n  WHEN \"Status\" IN ('awaiting_deliver','acceptance_in_progress') THEN 'HandedOver'\r\n  WHEN \"Status\" IN ('delivering','driver_pickup') THEN 'InTransit'\r\n  WHEN \"Status\" IN ('arbitration','client_arbitration','not_accepted') THEN 'Problem'\r\n  WHEN \"Status\" IN ('cancelled','cancelled_from_split_pending') THEN 'Cancelled'\r\n  ELSE 'Unknown' END,\r\n \"Substatus\" = CASE\r\n  WHEN \"Substatus\" IN ('posting_delivered','posting_received','posting_conditionally_delivered') THEN 'Delivered'\r\n  WHEN \"Substatus\" IN ('posting_created','posting_split_pending','posting_awaiting_registration','posting_registered','posting_awaiting_passport_data') THEN 'Assembling'\r\n  WHEN \"Substatus\" IN ('posting_acceptance_in_progress','posting_transferred_to_courier_service','posting_in_carriage') THEN 'HandedOver'\r\n  WHEN \"Substatus\" IN ('posting_driver_pick_up','posting_on_way_to_city','posting_on_way_to_pickup_point','posting_in_courier_service','posting_in_pickup_point') THEN 'InTransit'\r\n  WHEN \"Substatus\" IN ('posting_in_arbitration','posting_in_client_arbitration','posting_registration_error','posting_not_in_carriage','posting_not_in_sort_center','ship_failed') THEN 'Problem'\r\n  WHEN \"Substatus\" IN ('posting_canceled','posting_returned_to_warehouse') THEN 'Cancelled'\r\n  ELSE 'Unknown' END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
