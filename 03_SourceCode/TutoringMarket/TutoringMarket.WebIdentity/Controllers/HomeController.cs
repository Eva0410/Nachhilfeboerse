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
using TutoringMarket.Persistence;
using System.Net.Mail;

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

        public ActionResult ModalPopUp()
        {
            return View();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("orascanin.99@gmail.com"));
                message.From = new MailAddress("diplomarbeitdanijal@gmail.com");
                message.Subject = "Anfrage!";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Nachricht);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "diplomarbeitdanijal@gmail.com",
                        Password = ".di,wx,01,21"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Sent");
                }
            }
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
        public async Task<IActionResult> GetTutor()
        {
            GetTutorModel model = new GetTutorModel();
            await model.Init(this.uow, this.um, User.Identity.Name);
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> GetTutor(GetTutorModel model)
        {
            model.Tutor.IdentityName = User.Identity.Name;

            if (ModelState.IsValid && model.SelectedSubjects != null)
            {
                List<Subject> subjects = new List<Subject>();
                foreach (var item in model.SelectedSubjects)
                {
                    subjects.Add(uow.SubjectRepository.GetById(item));
                }
                model.Tutor.Subjects = subjects;
                model.Tutor.Accepted = false;
                this.uow.TutorRepository.Insert(model.Tutor);
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
                if (model.SelectedSubjects == null)
                {
                    ModelState.AddModelError("SelectedSubjects", "Bitte wähle deine Fächer aus!");
                }

                await model.FillList(uow, this.um, User.Identity.Name);
                return View(model);
            }
        }
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> EditTutor()
        {
            EditTutorModel model = new EditTutorModel();
            await model.FillList(uow, um, User);
            return View(model);
        }
        [Authorize(Roles = "Tutor")]
        [HttpPost]
        public async Task<IActionResult> EditTutor(EditTutorModel model)
        {
            var errors = ModelState.Select(e => e.Value.Errors).Where(v => v.Count > 0).ToList(); //for debugging
            if (ModelState.IsValid)
            {
                var oldTutor = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name && t.OldTutorId != 0, includeProperties: "Subjects").FirstOrDefault();
                if (oldTutor == null)
                    oldTutor = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name, includeProperties: "Subjects").FirstOrDefault();
                if (oldTutor != null)
                {
                    var changed = GetChangedProperties(oldTutor, model.Tutor);
                    Tutor changedTutor = new Tutor();
                    GenericRepository<Tutor>.CopyProperties(changedTutor, oldTutor);

                    //overrite changed properties
                    foreach (var item in changed)
                    {
                        var prop = oldTutor.GetType().GetProperty(item);
                        var value = model.Tutor.GetType().GetProperty(item).GetValue(model.Tutor, null);
                        prop.SetValue(changedTutor, value);
                    }
                    changedTutor.Subjects = new List<Subject>();
                    foreach (var item in model.SelectedSubjects)
                    {
                        changedTutor.Subjects.Add(uow.SubjectRepository.GetById(item));
                    }
                    //For updating the references, a new tutor has to be created and inserted; the old tutor will be deleted
                    changedTutor.Id = 0;
                    if (oldTutor.Accepted == true)
                    {
                        changedTutor.Accepted = false;
                        changedTutor.OldTutorId = oldTutor.Id;
                    }
                    else
                    {
                        this.uow.TutorRepository.Delete(oldTutor.Id);
                        this.uow.Save();
                    }

                    this.uow.TutorRepository.Insert(changedTutor);
                    this.uow.Save();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            else
            {
                if (model.SelectedSubjects == null)
                {
                    ModelState.AddModelError("SelectedSubjects", "Bitte wähle deine Fächer aus!");
                }
                await model.FillList(uow, um, User);
                return View(model);
            }

        }
        [HttpPost]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> DeleteTutoringProfile()
        {
            var tutor = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name).FirstOrDefault();
            if (tutor == null)
                return NotFound();

            var editedTutor = this.uow.TutorRepository.Get(filter: t => t.OldTutorId == tutor.Id).FirstOrDefault();
            if (editedTutor != null)
            {
                this.uow.TutorRepository.Delete(editedTutor.Id);
            }
            this.uow.TutorRepository.Delete(tutor.Id);
            this.uow.Save();

            var user = await um.FindByNameAsync(User.Identity.Name);
            await um.AddToRoleAsync(user, "Visitor");
            await um.RemoveFromRoleAsync(user, "Tutor");

            //refresh cookie
            await sim.RefreshSignInAsync(user);

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
            await model.GetAdmins(um, this.uow);
            return View(model);
        }
        //TODO Info (Admin muss sich neu einloggen um Administrator-Rechte zu bekommen
        //TODO handle back button
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdministrationArea(AdministrationsAreaModel model)
        {
            ApplicationUser user = null;
            if (String.IsNullOrEmpty(model.NewAdmin))
            {
                ModelState.AddModelError("NewAdmin", "Bitte geben Sie einen Namen ein!");
            }
            else
            {
                user = await um.FindByNameAsync(model.NewAdmin);
            }
            await model.GetAdmins(um, this.uow);
            if (ModelState.IsValid && user != null)
            {
                await um.AddToRoleAsync(user, "Admin");
                //await sim.RefreshSignInAsync(user);
            }
            else
            {
                ModelState.AddModelError("NewAdmin", "Zu diesem Namen konnte kein Account gefunden werden! Achtung: Der Benutzer muss sich vorher einmal eingeloggt haben, bevor er zum Admin werden kann!");
                return View(model);
            }

            return RedirectToAction("AdministrationArea", model);
        }
        //TODO: Fehlermeldungen ausgeben
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdministrationAreaDelete(AdministrationsAreaModel model, string id)
        {
            var user = await um.FindByNameAsync(id);
            if (user != null)
            {
                await um.RemoveFromRoleAsync(user, "Admin");
                //await sim.RefreshSignInAsync(user);
            }
            await model.GetAdmins(um, this.uow);
            return RedirectToAction("AdministrationArea", model);
        }
        //TODO Access Denied Page designen
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
            if (ModelState.IsValid && !String.IsNullOrEmpty(model.NewSubject.Name) && uow.SubjectRepository.Get(filter: s => s.Name == model.NewSubject.Name).FirstOrDefault() == null)
            {
                uow.SubjectRepository.Insert(model.NewSubject);
                uow.Save();
                return RedirectToAction("EditMetadata");
            }
            else
            {
                if (String.IsNullOrEmpty(model.NewSubject.Name))
                    ModelState.AddModelError("NewSubject.Name", "Die Bezeichnung darf nicht leer sein!");
                else if (uow.SubjectRepository.Get(filter: s => s.Name == model.NewSubject.Name).FirstOrDefault() != null)
                    ModelState.AddModelError("NewSubject.Name", "Dieses Fach existiert bereits!");
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
        [Authorize(Roles = "Admin")]
        public IActionResult EditTutors()
        {
            EditTutorsModel model = new EditTutorsModel();
            model.Init(this.uow);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTutorsDelete(EditTutorsModel model, int id)
        {
            var t = uow.TutorRepository.GetById(id);
            if (t == null)
                return NotFound();
            var user = await um.FindByNameAsync(t.IdentityName);
            if (user != null)
            {
                foreach (var item in this.uow.TeacherCommentRepository.Get(filter: tu => tu.Tutor_Id == t.Id))
                {
                    this.uow.TeacherCommentRepository.Delete(item.Id);
                }
                uow.Save();

                uow.TutorRepository.Delete(t);
                uow.Save();

                if (uow.TutorRepository.Get(filter: tut => tut.IdentityName == t.IdentityName).Count() == 0)
                {
                    await um.AddToRoleAsync(user, "Visitor");
                    await um.RemoveFromRoleAsync(user, "Tutor");
                    //await sim.SignInAsync(user,false);
                }

            }

            return RedirectToAction("EditTutors");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditTutorsAccept(EditTutorsModel model, int id)
        {
            var refreshedTutor = uow.TutorRepository.GetById(id);
            if (refreshedTutor == null)
                return NotFound();

            uow.TutorRepository.Delete(refreshedTutor.OldTutorId);
            uow.Save();

            refreshedTutor.Accepted = true;
            uow.TutorRepository.Update(refreshedTutor);

            uow.Save();

            foreach (var item in this.uow.TeacherCommentRepository.Get(filter: t => t.Tutor_Id == refreshedTutor.Id))
            {
                this.uow.TeacherCommentRepository.Delete(item.Id);
            }
            uow.Save();
            return RedirectToAction("EditTutors");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTutorsDeleteProfile(EditTutorsModel model, int id)
        {
            var t = uow.TutorRepository.GetById(id);
            if (t == null)
                return NotFound();
            var user = await um.FindByNameAsync(t.IdentityName);
            if (user != null)
            {
                uow.TutorRepository.Delete(t);
                uow.Save();

                await um.AddToRoleAsync(user, "Visitor");
                await um.RemoveFromRoleAsync(user, "Tutor");

                return RedirectToAction("EditTutors");
            }
            return NotFound();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditTutorsDeleteSubject(EditTutorsModel model, int tid, int subid)
        {
            var tutor = uow.TutorRepository.Get(filter: t => t.Id == tid, includeProperties:"Subjects").FirstOrDefault();
            if (tutor == null)
                return NotFound();
            if (tutor.Subjects.Count == 1)
            {
                model.Init(this.uow);
                ModelState.AddModelError("Error", "Jeder Tutor muss mindestens ein Fach haben, dieser Tutor hat nur mehr ein Fach; bitte löschen Sie den Tutor als Ganzen!");
                return View("EditTutors", model);
            }
            else
            {
                Tutor newTutor = new Tutor();
                GenericRepository<Tutor>.CopyProperties(newTutor, tutor);
                newTutor.Id = 0;
                newTutor.Subjects = new List<Subject>();
                foreach (var item in tutor.Subjects)
                {
                    if (item.Id != subid)
                    {
                        newTutor.Subjects.Add(item);
                    }
                }

                uow.TutorRepository.Delete(tutor);
                uow.TutorRepository.Insert(newTutor);
                uow.Save();

                return RedirectToAction("EditTutors");
            }
        }
        [Authorize(Roles = "Teacher")]
        public IActionResult CommentTutor()
        {
            CommentTutorModel model = new CommentTutorModel();
            model.Init(this.uow);
            return View(model);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult CommentTutor(CommentTutorModel model)
        {
            if (!String.IsNullOrEmpty(model.Comment))
            {
                TeacherComment comment = new TeacherComment
                {
                    Comment = model.Comment,
                    Tutor_Id = model.Tutor_Id,
                    TeacherIdentityName = User.Identity.Name
                };
                this.uow.TeacherCommentRepository.Insert(comment);
                this.uow.Save();
                model.Init(this.uow);
                return RedirectToAction("CommentTutor", model);
            }
            else
            {
                model.Init(this.uow);
                ModelState.AddModelError("Error", "Bitte geben Sie einen Kommentar ein!");
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
        public ActionResult Sent()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
