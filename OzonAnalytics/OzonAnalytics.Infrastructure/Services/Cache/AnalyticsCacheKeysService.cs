namespace OzonAnalytics.Infrastructure.Services.Cache
{
    public static class AnalyticsCacheKeysService
    {
        private static readonly string Prefix = "analytics";
        public static string Version(Guid shopId) => $"{Prefix}:{shopId}:version";
        public static string Data(Guid shopId, long version, string method, string parametr) => $"{Prefix}:{shopId}:version{version}:{method}:{parametr}";
    }
}
