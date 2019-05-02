using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleMVCCore.Models;
using System.Security.Claims;
using UW.Shibboleth;

namespace SampleMVCCore.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var vm = new HomeViewModel();
            var ident = (ClaimsIdentity)HttpContext.User.Identity;

            vm.NetID = ident.FindFirst(UWShibbolethClaimsType.UID).Value;
            vm.wiscEduPVI = ident.FindFirst(UWShibbolethClaimsType.PVI).Value;

            return View(vm);
        }
    }
}