using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;

namespace JobTimer.WebApplication.Security
{
    public class ExternalLoginData
    {
        public string LoginProvider { get; private set; }
        public string ProviderKey { get; private set; }
        public string Email { get; private set; }
        public string ExternalAccessToken { get; private set; }

        public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
        {
            Claim providerKeyClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);

            if (providerKeyClaim == null || string.IsNullOrEmpty(providerKeyClaim.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value))
            {
                return null;
            }

            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
            {
                return null;
            }

            var accessToken = identity.FindFirstValue("ExternalAccessToken");
            var email = identity.FindFirstValue(ClaimTypes.Email);
            
            return new ExternalLoginData
            {
                LoginProvider = providerKeyClaim.Issuer,
                ProviderKey = providerKeyClaim.Value,
                Email = email,
                ExternalAccessToken = accessToken
            };
        }
    }

}