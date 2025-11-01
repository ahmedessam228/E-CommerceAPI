using Domain.Interfaces.Service;
using Microsoft.Extensions.Caching.Memory;
using Shared.Helpers;

namespace Service
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<string> GenerateOtpAsync(string email, bool isForRegistration = false)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            if (isForRegistration)
            {
                // Registration OTP (single-use)
                _cache.Set($"{email}_Verified", new OtpData
                {
                    Code = otp,
                    Expiry = DateTime.UtcNow.AddMinutes(10)
                }, TimeSpan.FromMinutes(10));
            }
            else
            {
                // Password reset OTP (allows multiple checks)
                _cache.Set($"{email}_Verified", new OtpData
                {
                    Code = otp,
                    RemainingChecks = 2, // Only needed for password reset
                    Expiry = DateTime.UtcNow.AddMinutes(10)
                }, TimeSpan.FromMinutes(10));
            }
            return otp;
        }

        public async Task RemoveOtpAsync(string email)
        {
            _cache.Remove(email);
        }
    }
}
