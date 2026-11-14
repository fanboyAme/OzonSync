using OzonAnalytics.Domain.Enums;

namespace OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto
{
    public record class SummaryResponseDto(int OrderCount, int ItemsSold, decimal Revenue, decimal Commission, decimal Payout, decimal AverageCheck, List<StatusCountDto> StatusBreakdown);

    public record class StatusCountDto(OrderStatusAndSubStatus Status, int Count);
}



