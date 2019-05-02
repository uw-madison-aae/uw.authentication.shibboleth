using SampleMVCNet47.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using UW.Shibboleth;

namespace SampleMVCNet47.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {

            var vm = new HomeViewModel();
            var ident = (ClaimsIdentity)HttpContext.User.Identity;

            vm.NetID = ident.FindFirst(UWShibbolethClaimsType.UID).Value;
            vm.wiscEduPVI = ident.FindFirst(UWShibbolethClaimsType.PVI).Value;

            return View(vm);
        }
    }
}