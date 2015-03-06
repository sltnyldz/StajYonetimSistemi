﻿//----------------------------------------------------------------------------------------------
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Web.Mvc;

namespace TrainingPoint
{
    public partial class Startup
    {
        private DataAccess db = new DataAccess();
        public void ConfigureAuth(IAppBuilder app)
        {
            string ClientId = ConfigurationManager.AppSettings["ida:ClientID"];
            string Password = ConfigurationManager.AppSettings["ida:Password"];
            string Authority = string.Format(ConfigurationManager.AppSettings["ida:Authority"], "common");
            string GraphAPIIdentifier = ConfigurationManager.AppSettings["ida:GraphAPIIdentifier"];
                        
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions { });
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = ClientId,
                    Authority = Authority,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                        // map the claimsPrincipal's roles to the roles claim
                        RoleClaimType = "roles",
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        RedirectToIdentityProvider = (context) =>
                        {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                            // this allows you to deploy your app (to Azure Web Sites, for example) without having to change settings
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                            //string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                            context.ProtocolMessage.PostLogoutRedirectUri = new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Index", "Home", null, HttpContext.Current.Request.Url.Scheme);
                            context.ProtocolMessage.Resource = GraphAPIIdentifier;
                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = (context) =>
                        {
                            ClientCredential credential = new ClientCredential(ClientId, Password);
                            string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                            AuthenticationContext authContext = new AuthenticationContext(string.Format("https://login.windows.net/{0}", tenantID), 
                                new ADALTokenCache(signedInUserID));
                            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                                context.Code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential);

                            return Task.FromResult(0);
                        },
                        // we use this notification for injecting our custom logic
                        SecurityTokenValidated = (context) =>
                        {
                            // retriever caller data from the incoming principal
                            string issuer = context.AuthenticationTicket.Identity.FindFirst("iss").Value;
                            if (db.Organizations.FirstOrDefault(a => (a.Issuer == issuer)) == null)
                                // the caller is not from a trusted issuer - throw to block the authentication flow
                                throw new System.IdentityModel.Tokens.SecurityTokenValidationException();

                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            context.OwinContext.Response.Redirect(new UrlHelper(HttpContext.Current.Request.RequestContext).
                                Action("Index", "Home", null, HttpContext.Current.Request.Url.Scheme));
                            context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}