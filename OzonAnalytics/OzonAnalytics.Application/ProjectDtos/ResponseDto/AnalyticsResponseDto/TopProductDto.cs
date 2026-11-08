namespace OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto
{
    public record class TopProductDto(Guid ProductId, long OzonSku, string Name, int ItemsSold, decimal Revenue, decimal Payout);
}
