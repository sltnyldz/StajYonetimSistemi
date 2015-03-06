using StajYonetim.Models;
using System.Security.Claims;
using System.Web.Mvc;

namespace StajYonetim.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var userProfile = new UserProfile();
            if (Request.IsAuthenticated)
            {
                var user = ClaimsPrincipal.Current.Identity;
                userProfile.DisplayName = user.Name;
            }

            return View(userProfile);

        }

        // Asagidaki kodu ornek alarak kullanicinin hangi grupta oldugu bilgisine ulasicaz
        // Bu bilgiye ulasmak icin GraphAPI denilen Azure Active Directory'nin APIsini cagirmamiz gerekecek...

        //private const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
        //private const string LoginUrl = "https://login.windows.net/{0}";
        //private const string GraphUrl = "https://graph.windows.net";
        //private const string GraphUserUrl = "https://graph.windows.net/{0}/users/{1}?api-version=2013-04-05";
        //private static readonly string AppPrincipalId = ConfigurationManager.AppSettings["ida:ClientID"];
        //private static readonly string AppKey = ConfigurationManager.AppSettings["ida:Password"];

        //[Authorize]
        //public async Task<ActionResult> UserProfile()
        //{
        //    string tenantId = ClaimsPrincipal.Current.FindFirst(TenantIdClaimType).Value;

        //    // Get a token for calling the Windows Azure Active Directory Graph
        //    AuthenticationContext authContext = new AuthenticationContext(String.Format(CultureInfo.InvariantCulture, LoginUrl, tenantId));
        //    ClientCredential credential = new ClientCredential(AppPrincipalId, AppKey);
        //    AuthenticationResult assertionCredential = authContext.AcquireToken(GraphUrl, credential);
        //    string authHeader = assertionCredential.CreateAuthorizationHeader();
        //    string requestUrl = String.Format(
        //        CultureInfo.InvariantCulture,
        //        GraphUserUrl,
        //        HttpUtility.UrlEncode(tenantId),
        //        HttpUtility.UrlEncode(User.Identity.Name));

        //    HttpClient client = new HttpClient();
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        //    request.Headers.TryAddWithoutValidation("Authorization", authHeader);
        //    HttpResponseMessage response = await client.SendAsync(request);
        //    string responseString = await response.Content.ReadAsStringAsync();
        //    UserProfile profile = JsonConvert.DeserializeObject<UserProfile>(responseString);

        //    return View(profile);
        //}
    }
}
