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
using Microsoft.AspNetCore.Http;
using System.Net;

namespace TutoringMarket.WebIdentity.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork uow;
        UserManager<ApplicationUser> um;
        SignInManager<ApplicationUser> sim;
        public HomeController(IUnitOfWork _uow, UserManager<ApplicationUser> _um, SignInManager<ApplicationUser> _sm)
        {
            uow = _uow;
            um = _um;
            sim = _sm;
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
        [Authorize(Roles = "Visitor")]
        public IActionResult GetTutor()
        {
            GetTutorModel model = new GetTutorModel();
            model.Init(this.uow);
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetTutor(GetTutorModel model)
        {
            model.Tutor.IdentityName = User.Identity.Name;

            if (ModelState.IsValid && model.SelectedSubjects != null)
            {
                this.uow.TutorRepository.Insert(model.Tutor);
                this.uow.Save();
                foreach (var item in model.SelectedSubjects)
                {
                    Tutor_Subject ts = new Tutor_Subject();
                    ts.Subject_Id = item;
                    ts.Tutor_Id = model.Tutor.Id;
                    this.uow.TutorSubjectRepository.Insert(ts);
                }
                this.uow.Save();

                var user = await um.FindByNameAsync(User.Identity.Name);
                await um.AddToRoleAsync(user, "Tutor");
                await um.RemoveFromRoleAsync(user, "Visitor");

                //the cookie must be refreshed
                await sim.SignOutAsync();
                await sim.SignInAsync(user, true);

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("SelectedSubjects", "Bitte wähle deine Fächer aus!");
                var errors = ModelState.Select(e => e.Value.Errors).Where(v => v.Count > 0).ToList(); //for debugging
                model.FillList(uow);
                return View(model);
            }
        }
        [Authorize(Roles ="Tutor")]
        public IActionResult EditTutor()
        {
            EditTutorModel model = new EditTutorModel();
            model.FillList(uow, User);
            return View(model);
        }
        [Authorize(Roles ="Tutor")]
        [HttpPost]
        public IActionResult EditTutor(EditTutorModel model)
        {
            var errors = ModelState.Select(e => e.Value.Errors).Where(v => v.Count > 0).ToList(); //for debugging
            if (ModelState.IsValid)
            {
                var oldTutor = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name).FirstOrDefault();
                var changed = GetChangedProperties(oldTutor, model.Tutor);
                //overrite changed properties
                foreach (var item in changed)
                {
                    var prop = oldTutor.GetType().GetProperty(item);
                    var value = model.Tutor.GetType().GetProperty(item).GetValue(model.Tutor,null);
                    prop.SetValue(oldTutor,value);
                }

                var oldTutorSubjectIds = this.uow.TutorSubjectRepository.Get(ts => ts.Tutor_Id == oldTutor.Id).Select(ts => ts.Id).ToList();
                //delete old Tutor_Subjects
                foreach (var item in oldTutorSubjectIds)
                {
                    this.uow.TutorSubjectRepository.Delete(item);
                }
                //add new Tutor_Subjects
                foreach (var item in model.SelectedSubjects)
                {
                    Tutor_Subject ts = new Tutor_Subject();
                    ts.Tutor_Id = oldTutor.Id;
                    ts.Subject_Id = item;
                    this.uow.TutorSubjectRepository.Insert(ts);
                }
                this.uow.TutorRepository.Update(oldTutor);
                this.uow.Save();
                return RedirectToAction("Index");
            }
            else
            {
                model.FillList(uow, User);
                return View(model);
            }
            
        }
        [HttpPost]
        [Authorize(Roles ="Tutor")]
        public async Task<IActionResult> DeleteTutoringProfile()
        {
            var tutor = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name).FirstOrDefault();
            if (tutor == null)
                return NotFound();
            this.uow.TutorRepository.Delete(tutor.Id);
            this.uow.Save();

            var user = await um.FindByNameAsync(User.Identity.Name);
            await um.AddToRoleAsync(user, "Visitor");
            await um.RemoveFromRoleAsync(user, "Tutor");

            //the cookie must be refreshed
            await sim.SignOutAsync();
            await sim.SignInAsync(user, true);

            return RedirectToAction("Index");
        }
        private List<String> GetChangedProperties(Tutor oldTutor, Tutor newTutor)
        {
            List<String> changed = new List<string>();
            List<String> props = new List<string>(typeof(Tutor).GetProperties().Select(p => p.Name).ToList());
            props.Remove("Timestamp");
            props.Remove("Id");

            foreach (var item in props)
            {
                    if (oldTutor.GetType().GetProperty(item).GetValue(oldTutor, null)?.ToString() != (newTutor.GetType().GetProperty(item).GetValue(newTutor, null)?.ToString()) && (newTutor.GetType().GetProperty(item).GetValue(newTutor, null)?.ToString()) != null)
                    {
                        changed.Add(item);
                    }
            }
            return changed;

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
            await model.GetAdmins(um);
            if (ModelState.IsValid && user != null)
            {
                await um.AddToRoleAsync(user, "Admin");
            }
            else
            {
                ModelState.AddModelError("NewAdmin", "Zu diesem Namen konnte kein Account gefunden werden! Achtung: Der Benutzer muss sich vorher einmal eingeloggt haben, bevor er zum Admin werden kann!");
                return View(model);
            }

            return View(model);
        }
        //TODO: Fehlermeldungen ausgeben
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdministrationAreaDelete(AdministrationsAreaModel model, string id)
        {
            var user = await um.FindByNameAsync(id);
            if (user != null)
            {
                await um.RemoveFromRoleAsync(user, "Admin");
            }
            await model.GetAdmins(um);
            return RedirectToAction("AdministrationArea", model);
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
        public IActionResult EditReviewsDelete(EditMetadataModel model, int id)
        {
            var r = uow.ReviewRepository.GetById(id);
            if (r == null)
                return NotFound();

            uow.ReviewRepository.Delete(r);
            uow.Save();
            return RedirectToAction("EditReviews");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditReviewsAccept(EditReviewsModel model, int id)
        {
            var review = uow.ReviewRepository.Get(filter: r => r.Id == id, includeProperties: "Tutor").FirstOrDefault();
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
        [Authorize(Roles = "Admin")]
        public IActionResult EditDepartmentsDelete(EditMetadataModel model, int id)
        {
            var dept = uow.DepartmentRepository.GetById(id);
            if (dept == null)
                return NotFound();

            uow.DepartmentRepository.Delete(dept);
            uow.Save();
            return RedirectToAction("EditMetadata");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditDepartments(EditMetadataModel model)
        {
            model.Init(uow);
            if (ModelState.IsValid && model.NewDepartment.Name != null)
            {
                uow.DepartmentRepository.Insert(model.NewDepartment);
                uow.Save();
                return RedirectToAction("EditMetadata");
            }
            else
            {
                ModelState.AddModelError("NewDepartment.Name", "Die Bezeichnunsg darf nicht leer sein!");
                return View("EditMetadata", model);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditClasses(EditMetadataModel model)
        {
            model.Init(uow);
            if (ModelState.IsValid && model.NewClass.Name != "")
            {
                uow.ClassRepository.Insert(model.NewClass);
                uow.Save();
                return RedirectToAction("EditMetadata");
            }
            else
            {
                ModelState.AddModelError("NewClass.Name", "Die Bezeichnung darf nicht leer sein!");
                return View("EditMetadata", model);
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditClassesDelete(EditMetadataModel model, int id)
        {
            var c = uow.ClassRepository.GetById(id);
            if (c == null)
                return NotFound();

            uow.ClassRepository.Delete(c);
            uow.Save();
            return RedirectToAction("EditMetadata");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditSubjects(EditMetadataModel model)
        {
            model.Init(uow);
            if (ModelState.IsValid && model.NewSubject.Name != "")
            {
                uow.SubjectRepository.Insert(model.NewSubject);
                uow.Save();
                return RedirectToAction("EditMetadata");
            }
            else
            {
                ModelState.AddModelError("NewSubject.Name", "Die Bezeichnung darf nicht leer sein!");
                return View("EditMetadata", model);
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditSubjectsDelete(EditMetadataModel model, int id)
        {
            var sub = uow.SubjectRepository.GetById(id);
            if (sub == null)
                return NotFound();

            uow.SubjectRepository.Delete(sub);
            uow.Save();
            return RedirectToAction("EditMetadata");
        }
        [Authorize(Roles ="Admin")]
        public IActionResult EditTutors()
        {
            EditTutorsModel model = new EditTutorsModel();
            model.Init(this.uow);
            return View(model);
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
