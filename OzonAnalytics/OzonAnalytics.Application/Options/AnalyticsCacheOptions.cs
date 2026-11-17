namespace OzonAnalytics.Application.Options
{
    public class AnalyticsCacheOptions
    {
        public const string Name = "Cache";
        public int AnalyticsTtlMinutes { get; set; } = 5;
    }
}