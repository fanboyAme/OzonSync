namespace OzonAnalytics.Application.Options
{
    public class SyncLockOptions
    {
        public const string Name = "Sync";
        public int LockTtlMinutes { get; set; } = 10;
    }
}
