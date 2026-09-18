namespace OzonAnalytics.Application.Interfaces.Shop
{
    public interface IEncryptionService
    {
        public string Protect(string key);

        public string Unprotect(string protectKey);
    }
}
