using System;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Web.Mvc;

namespace StajYonetim.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            RedirectResult result;
            if (!Request.IsAuthenticated)
            {
                string uniqueId = "";
                string returnUrl = HttpContext.Request.RawUrl;
                bool rememberMe = false;

                SignInRequestMessage sirm = FederatedAuthentication.WSFederationAuthenticationModule.CreateSignInRequest(
                    uniqueId, returnUrl, rememberMe);
                result = Redirect(sirm.RequestUrl);
            }
            else
            {
                result = Redirect("~/");
            }
            result.ExecuteResult(ControllerContext);
        }

        public ActionResult SignOut()
        {
            WsFederationConfiguration config = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration;
            // Redirect to SignOutCallback after signing out.
            string callbackUrl = Url.Action("Index", "Home", routeValues: null, protocol: Request.Url.Scheme);

            SignOutRequestMessage signoutMessage = new SignOutRequestMessage(new Uri(config.Issuer), callbackUrl);
            signoutMessage.SetParameter("wtrealm", IdentityConfig.Realm ?? config.Realm);

            FederatedAuthentication.SessionAuthenticationModule.SignOut();

            return new RedirectResult(signoutMessage.WriteQueryString());
        }
    }
}
