﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TutoringMarket.WebIdentity.Models.ViewModels;
using TutoringMarket.Core.Contracts;

namespace TutoringMarket.WebIdentity.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork uow;
        public HomeController(IUnitOfWork _uow)
        {
            uow = _uow;
        }
        [Authorize]
        public IActionResult Index(String SortTextBefore)
        {
            IndexModel model = new IndexModel();
            model.FillTutors(uow);
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Index(IndexModel model)
        {
            ModelState.Clear();
            model.FillTutors(uow);
            return View(model);
        }
        [Authorize]
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

        public IActionResult GetTutor()
        {
            ViewData["Message"] = "Your tutor page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
