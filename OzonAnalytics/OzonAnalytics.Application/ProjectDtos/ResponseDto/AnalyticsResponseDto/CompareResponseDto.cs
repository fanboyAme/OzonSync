namespace OzonAnalytics.Application.ProjectDtos.ResponseDto.AnalyticsResponseDto
{
    public record class CompareResponseDto(SummaryResponseDto Current, SummaryResponseDto Previous, DeltaSummary Delta);
    public record class DeltaSummary(int OrderCountDiff, decimal RevenueDiff, decimal PayoutDiff, decimal? OrderCountDiffPercent, decimal? RevenueDiffPercent, decimal? PayoutDiffPercent);
}
