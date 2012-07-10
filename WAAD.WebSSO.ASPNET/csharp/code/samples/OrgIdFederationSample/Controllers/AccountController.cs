namespace OrgIdFederationSample.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Microsoft.IdentityModel.Web;

    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            WSFederationAuthenticationModule fam = FederatedAuthentication.WSFederationAuthenticationModule;
            FormsAuthentication.SignOut();
            fam.SignOut(true);
            return new RedirectResult(string.Format("https://accounts.accesscontrol.windows.net/v2/wsfederation?wa=wsignout1.0&wreply={0}", Url.Encode("http://localhost/OrgIdFederationSample")));
        }
    }
}
