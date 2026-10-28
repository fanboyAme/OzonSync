using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Infrastructure.Ozon.Models
{
    public record OzonKeyValidationResult(bool IsValid, DateTime? ExpiresAt);
}
