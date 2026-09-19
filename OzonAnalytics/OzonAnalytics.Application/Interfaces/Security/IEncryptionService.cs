namespace OzonAnalytics.Application.Interfaces.Security
{
    public interface IEncryptionService
    {
        public string Protect(string key);

        public string Unprotect(string protectKey);
    }
}
