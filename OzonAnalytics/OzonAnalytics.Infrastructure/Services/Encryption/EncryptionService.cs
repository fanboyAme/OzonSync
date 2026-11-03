using Microsoft.AspNetCore.DataProtection;
using OzonAnalytics.Application.Interfaces.Security;

namespace OzonAnalytics.Infrastructure.Services.EncryptedService
{

    public class EncryptionService : IEncryptionService
    {
        private readonly IDataProtector _dataProtector;
        public EncryptionService(IDataProtectionProvider provider)
        {
            _dataProtector = provider.CreateProtector("OzonApiKeyProtection");
        }
        public string Protect(string key) => _dataProtector.Protect(key);

        public string Unprotect(string protectKey) => _dataProtector.Unprotect(protectKey);

    }
}
