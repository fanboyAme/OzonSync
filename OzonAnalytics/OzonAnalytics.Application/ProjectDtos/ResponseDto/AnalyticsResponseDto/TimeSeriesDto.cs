namespace OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto
{
    public record class TimeSeriesDto(DateTime Date, int OrderCount, decimal Revenue, decimal Payout);
}
