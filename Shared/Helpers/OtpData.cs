
namespace Shared.Helpers
{
    public class OtpData
    {
        public string Code { get; set; }
        public int RemainingChecks { get; set; }
        public DateTime Expiry { get; set; }
    }
}
