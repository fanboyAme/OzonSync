using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Services.EncryptedService
{

    public class EncryptionService
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
