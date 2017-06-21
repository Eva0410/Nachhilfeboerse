using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TutoringMarket.WebIdentity.Models.ViewModels;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;
using Microsoft.AspNetCore.Identity;
using TutoringMarket.WebIdentity.Models;

namespace TutoringMarket.WebIdentity.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork uow;
        UserManager<ApplicationUser> um;
        public HomeController(IUnitOfWork _uow, UserManager<ApplicationUser> _um)
        {
            uow = _uow;
            um = _um;
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
        public IActionResult TutorDetails(int id)
        {
            TutorModel model = new TutorModel();
            if (uow.TutorRepository.GetById(id) == null)
                return NotFound();
            model.Init(uow, id);
            return View(model);
        }
       [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdministrationArea()
        {
            
            AdministrationsAreaModel model = new AdministrationsAreaModel();
            await model.GetAdmins(um);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdministrationArea(AdministrationsAreaModel model)
        {
            var user = await um.FindByNameAsync(model.NewAdmin);
            if(user != null)
            {
                await um.AddToRoleAsync(user, "Admin");
            }
            await model.GetAdmins(um);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditReviews()
        {
            EditReviewsModel model = new EditReviewsModel();
            model.Init(uow);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("EditReviewsDelete")]
        public IActionResult EditReviewsDelete(EditReviewsModel model)
        {
            var review = uow.ReviewRepository.GetById(model.ReviewId);
            if (review == null)
                return NotFound();

            uow.ReviewRepository.Delete(review);
            uow.Save();
            return RedirectToAction("EditReviews");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteReview()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditReviews(EditReviewsModel model,bool EditReviewsSave)
        {
            var review = uow.ReviewRepository.GetById(model.ReviewId);
            if (review == null)
                return NotFound();

            review.Approved = true;
            uow.ReviewRepository.Update(review);

            uow.Save();
            return RedirectToAction("EditReviews");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditMetadata()
        {
            EditMetadataModel model = new EditMetadataModel();
            model.Init(uow);
            return View(model);
        }
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public IActionResult EditMetadata(EditMetadataModel model)
        //{
        //    var dept = uow.DepartmentRepository.GetById(model.DepartmentId);
        //    if (dept == null)
        //        return NotFound();

        //    uow.DepartmentRepository.Delete(dept);
        //    uow.Save();
        //    return RedirectToAction("EditMetadata");
        //}
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditDepartments(EditMetadataModel model)
        {
            if (ModelState.IsValid) { 
                uow.DepartmentRepository.Insert(model.NewDepartment);
                uow.Save();
                return RedirectToAction("EditMetadata");
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditClasses(EditMetadataModel model)
        {
            if (ModelState.IsValid)
            {
                uow.ClassRepository.Insert(model.NewClass);
                uow.Save();
                return RedirectToAction("EditMetadata");
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditSubjects(EditMetadataModel model)
        {
            if (ModelState.IsValid)
            {
                uow.SubjectRepository.Insert(model.NewSubject);
                uow.Save();
                return RedirectToAction("EditMetadata");
            }
            else
            {
                return View(model);
            }
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
