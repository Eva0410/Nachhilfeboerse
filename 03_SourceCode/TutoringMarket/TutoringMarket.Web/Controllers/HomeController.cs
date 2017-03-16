using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TutoringMarket.Core.Contracts;
using Authentication;

namespace TutoringMarket.Web.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork _uow;
        public HomeController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index()
        {
            string user = "in130021";
            string pw = "yuchdybu";

            int? n = CheckLogin(user, pw).Result;

            //var li = _uow.TutorRepository.Get().ToList();
            return View();
        }
        async Task<int?> CheckLogin(string user, string pw)
        {
            int? num = null;
            if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(pw))
            {
                SVSAuthenticationSoapClient client = new SVSAuthenticationSoapClient(SVSAuthenticationSoapClient.EndpointConfiguration.SVSAuthenticationSoap);
                num = await client.CheckLdapSchuelerLoginEdvoAsync(user, pw);
            }
            return num;
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
