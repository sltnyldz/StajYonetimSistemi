using StajYonetim.Models;
using System.Linq;
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
                string displayName = ClaimsPrincipal.Current.Claims.First(c => c.Type.ToString().Contains("displayname")).Value;
                userProfile.DisplayName = displayName;

                //GraphAPI'ye request
            }

            return View(userProfile);

        }

        public ActionResult Ara()
        {
            return View();
        }

    }
}
