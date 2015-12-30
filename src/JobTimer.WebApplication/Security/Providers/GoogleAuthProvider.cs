﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Threading.Tasks;
using System.Security.Claims;

namespace JobTimer.WebApplication.Security.Providers
{
    public class GoogleAuthProvider : IGoogleOAuth2AuthenticationProvider
    {
        public void ApplyRedirect(GoogleOAuth2ApplyRedirectContext context)
        {
            context.Response.Redirect(context.RedirectUri);
        }

        public Task Authenticated(GoogleOAuth2AuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            return Task.FromResult<object>(null);
        }

        public Task ReturnEndpoint(GoogleOAuth2ReturnEndpointContext context)
        {
            return Task.FromResult<object>(null);
        }
    }
}