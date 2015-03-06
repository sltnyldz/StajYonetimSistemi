using System.IdentityModel.Tokens;
using System.Linq;

namespace StajYonetim.Utils
{
    public class DatabaseIssuerNameRegistry : ValidatingIssuerNameRegistry
    {
        public static bool ContainsTenant(string tenantId)
        {
            return tenantId == "7cdc7bb2-d47a-4021-9b85-34dd865089e5";
        }

        public static bool ContainsKey(string thumbprint)
        {
            return thumbprint == "3270BF5597004DF339A4E62224731B6BD82810A6";
        }

        private static string GetIssuerId(string issuer)
        {
            return issuer.TrimEnd('/').Split('/').Last();
        }

        protected override bool IsThumbprintValid(string thumbprint, string issuer)
        {
            return ContainsTenant(GetIssuerId(issuer))
                && ContainsKey(thumbprint);
        }
    }
}
