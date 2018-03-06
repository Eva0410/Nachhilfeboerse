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
using System.IO;
using System.Drawing;

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
        //TODO sort property mitgeben
        [Authorize]
        public IActionResult Index(string filter, string sort)
        {
            IndexModel model = new IndexModel();
            model.SelectedSortProperty = sort;
            model.SelectedSubject = filter;
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
        [Authorize]
        //TODO config datei
        public async Task<ActionResult> Contact(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress("a.keck-jordan@htl-leonding.ac.at"));
                    message.From = new MailAddress("nachhilfeboerse.info@gmail.com"); //pw: 5nUtWnsz
                    message.Subject = "Anfrage!";
                    message.Body = string.Format(body, model.FromName, model.FromEmail, model.Nachricht);
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "nachhilfeboerse.info@gmail.com",
                            Password = "5nUtWnsz"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                        return RedirectToAction("Sent");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "Ein Fehler ist aufgetreten! Tipp: Haben Sie Ihre Verbindung zum Internet überprüft?");
                }
            }
            return View(model);
        }

        [Authorize]
        public IActionResult TutorDetails(int id, string filter, string sort)
        {
            TutorModel model = new TutorModel();
            if (uow.TutorRepository.GetById(id) == null)
                return NotFound();
            model.Tutor_Id = id;
            model.Init(uow);
            model.filter = filter;
            model.sort = sort;
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> TutorDetails(TutorModel model)
        {
            if(ModelState.IsValid)
            {
                Review r = model.NewReview;
                r.Approved = false;
                ApplicationUser user = await um.FindByNameAsync(User.Identity.Name);
                r.Author = String.Format("{0} {1}, {2}", user.FirstName, user.LastName, user.SchoolClass);
                r.Tutor_Id = model.Tutor_Id;
                r.Date = DateTime.Now;
                uow.ReviewRepository.Insert(r);
                uow.Save();
                model.Init(uow);
                return RedirectToAction("TutorDetails", model);
            }
            else
            {
                model.Init(uow);
                return View(model);
            }
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
        //id describes the method, which should be invoked (0 == preview image, 1 == save tutor, 2 == delete image)
        public async Task<IActionResult> GetTutor(GetTutorModel model, int id)
        {
            
            //preview
            if (id == 0)
            {
                if (ModelState.Where(k => k.Key == nameof(model.ImageFileName)).Count() == 0 || ModelState.Where(k => k.Key == nameof(model.ImageFileName)).FirstOrDefault().Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid)
                {
                    //save the filename of the picture, and the image as base64 string
                    model.ImageAsString = ConvertImage(model.Image, model.ImageAsString);
                }
                await model.FillList(uow, this.um, User.Identity.Name);
                return View(model);
            }
            else if (id == 1)
            {
                model.Tutor.IdentityName = User.Identity.Name;
                if (ModelState.IsValid && model.SelectedSubjects != null)
                {
                    //Save image
                    model.Tutor.Image = ConvertImage(model.Image, model.ImageAsString);

                    //add list of subjects to tutor
                    List<Subject> subjects = new List<Subject>();
                    foreach (var item in model.SelectedSubjects)
                    {
                        subjects.Add(uow.SubjectRepository.GetById(item));
                    }
                    model.Tutor.Subjects = subjects;

                    //Tutor is not accepted in the beginning
                    model.Tutor.Accepted = false;

                    this.uow.TutorRepository.Insert(model.Tutor);
                    this.uow.Save();

                    //update roles
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
                    //save image as string
                    model.ImageAsString = ConvertImage(model.Image, model.ImageAsString);
                    return View(model);
                }
            }
            else if (id == 2)
            {
                await model.FillList(uow, um, User.Identity.Name);
                model.ImageAsString = string.Empty;
                return View(model);
            }
            return NotFound();
        }
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> EditTutor()
        {
            EditTutorModel model = new EditTutorModel();
            await model.FillList(uow, um, User);
            if(model.Tutor.Image != null)
            {
                model.ImageAsString = model.Tutor.Image;
            }
            return View(model);
        }
        [Authorize(Roles = "Tutor")]
        [HttpPost]
        public async Task<IActionResult> EditTutor(EditTutorModel model, int id)
        {
            if (id == 0)
            {
                if (ModelState.Where(k => k.Key == nameof(model.ImageFileName)).Count() == 0 || ModelState.Where(k => k.Key == nameof(model.ImageFileName)).FirstOrDefault().Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid)
                {
                    //save the image as base64 string
                    model.ImageAsString = ConvertImage(model.Image, model.ImageAsString);
                }
                await model.FillList(uow, this.um, User);
                return View(model);
            }
            else if (id == 1)
            {
                var errors = ModelState.Select(e => e.Value.Errors).Where(v => v.Count > 0).ToList(); //for debugging
                if (ModelState.IsValid)
                {
                    //find the old version of the tutor
                    var oldTutor = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name && t.OldTutorId != 0, includeProperties: "Subjects").FirstOrDefault();
                    if (oldTutor == null)
                        oldTutor = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name, includeProperties: "Subjects").FirstOrDefault();
                    if (oldTutor != null) //in case the tutor is not in role tutor anymore (removed from admin), website will crash
                    {
                        var changed = GetChangedProperties(oldTutor, model.Tutor);
                        Tutor changedTutor = new Tutor();
                        //copy all other properties from old tutor
                        GenericRepository<Tutor>.CopyProperties(changedTutor, oldTutor);

                        //overrite changed properties
                        foreach (var item in changed)
                        {
                            var prop = oldTutor.GetType().GetProperty(item);
                            var value = model.Tutor.GetType().GetProperty(item).GetValue(model.Tutor, null);
                            prop.SetValue(changedTutor, value);
                        }
                        //update image
                        changedTutor.Image = ConvertImage(model.Image, model.ImageAsString);
                        //update subjects
                        changedTutor.Subjects = new List<Subject>();
                        foreach (var item in model.SelectedSubjects)
                        {
                            changedTutor.Subjects.Add(uow.SubjectRepository.GetById(item));
                        }
                        //For updating the references, a new tutor has to be created and inserted; the old tutor will be deleted
                        changedTutor.Id = 0;
                        changedTutor.Accepted = false;
                        //handling the accept/reject problem
                        if (oldTutor.Accepted == true)
                        {
                            changedTutor.OldTutorId = oldTutor.Id;
                        }
                        else
                        {
                            this.uow.TutorRepository.Delete(oldTutor.Id);
                            this.uow.Save();
                            var tmp = this.uow.TutorRepository.Get(t => t.IdentityName == User.Identity.Name && t.Accepted == true).FirstOrDefault();
                            if (tmp != null)
                                changedTutor.OldTutorId = tmp.Id;
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
                    //save image as string
                    model.ImageAsString = ConvertImage(model.Image, model.ImageAsString);
                    await model.FillList(uow, um, User);
                    return View(model);
                }
            }
            else if (id == 2)
            {
                await model.FillList(uow, um, User);
                model.ImageAsString = string.Empty;
                return View(model);
            }
            return NotFound();

        }
        public string ConvertImage(IFormFile Image, string ImageAsString)
        {
            if (Image != null && Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    return Convert.ToBase64String(fileBytes);
                }
            }
            return ImageAsString;
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
                //user will be refreshed after logging in again
            }
            await model.GetAdmins(um, this.uow);
            return RedirectToAction("AdministrationArea", model);
        }
        //TODO Access Denied Page designen
        //TODO Reviews im Nachhinein bearbeiten
        //TODO Max Anzahl Zeichen bei Kommentar
        //TODO reviews accept
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
        //Decline a tutor
        public async Task<IActionResult> EditTutorsDelete(EditTutorsModel model, int id)
        {
            var t = uow.TutorRepository.GetById(id);
            if (t == null)
                return NotFound();
            var user = await um.FindByNameAsync(t.IdentityName);
            if (user != null)
            {
                DeleteTutorComments(t.Id);

                uow.TutorRepository.Delete(t);
                uow.Save();

                if (uow.TutorRepository.Get(filter: tut => tut.IdentityName == t.IdentityName).Count() == 0)
                {
                    await um.AddToRoleAsync(user, "Visitor");
                    await um.RemoveFromRoleAsync(user, "Tutor");
                    //roles will be refreshed after the tutor has logged in again
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
            refreshedTutor.OldTutorId = 0;
            uow.TutorRepository.Update(refreshedTutor);

            uow.Save();

            DeleteTutorComments(refreshedTutor.Id);

            SendMail(refreshedTutor.EMail, String.Format("Hallo {0}, \r\n\r\n dein Profil wurde soeben vom Administrator freigegeben! Ab jetzt können alle Schüler dein Profil sehen und dich kontaktieren. \r\n\r\n Liebe Grüße \r\n Nachhilfebörse HTL Leonding", refreshedTutor.FirstName), "Profil freigegeben");

            return RedirectToAction("EditTutors");
        }
        private async void SendMail(string mail, string messageText, string subject)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(mail));
                    message.From = new MailAddress("nachhilfeboerse.info@gmail.com"); //pw: 5nUtWnsz
                    message.Subject = subject;
                    message.Body = string.Format(messageText);
                    message.IsBodyHtml = false;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "nachhilfeboerse.info@gmail.com",
                            Password = "5nUtWnsz"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "Ein Fehler ist aufgetreten! Tipp: Haben Sie Ihre Verbindung zum Internet überprüft?");
                }
            }
        }
        private void DeleteTutorComments(int tutor_id)
        {
            foreach (var item in this.uow.TeacherCommentRepository.Get(filter: t => t.Tutor_Id == tutor_id))
            {
                this.uow.TeacherCommentRepository.Delete(item.Id);
            }
            uow.Save();
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
                //user roles will be refreshed after the tutor logs in again

                return RedirectToAction("EditTutors");
            }
            return NotFound();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditTutorsDeleteSubject(EditTutorsModel model, int tid, int subid)
        {
            var tutor = uow.TutorRepository.Get(filter: t => t.Id == tid, includeProperties: "Subjects").FirstOrDefault();
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
                //to update the subjects, the tutor must be inserted again
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
        public IActionResult CommentTutor(string filter)
        {
            CommentTutorModel model = new CommentTutorModel();
            model.SelectedSubject = filter;
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
                return RedirectToAction("CommentTutor", new { filter = model.SelectedSubject });
            }
            else
            {
                model.Init(this.uow);
                ModelState.AddModelError("Error", "Bitte geben Sie einen Kommentar ein!");
                return View(model);
            }
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult CommentTutorFilter(CommentTutorModel model)
        {
            model.Init(this.uow);
            return View("CommentTutor", model);
        }
        [Authorize]
        public async Task<IActionResult> TutorRequest(int id)
        {
            TutorRequestModel model = new TutorRequestModel();
            await model.Init(this.uow, id, this.um, User.Identity.Name);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> TutorRequest(TutorRequestModel model)
        {
            model.InitTutor(this.uow, model.Tutor.Id);
            if (!String.IsNullOrEmpty(model.Message))
            {
                try
                {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(model.Tutor.EMail));
                    message.From = new MailAddress("nachhilfeboerse.info@gmail.com"); //pw: 5nUtWnsz
                    message.Subject = "Nachhilfeanfrage!";
                    message.Body = model.Message + model.Comments + model.Ending;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "nachhilfeboerse.info@gmail.com",
                            Password = "5nUtWnsz"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                        return RedirectToAction("Sent");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Message", "Ein Fehler ist aufgetreten! Tipp: Haben Sie Ihre Verbindung zum Internet überprüft?");
                }
            }
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
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> AdministrationMail(int id)
        {
            AdminMailForm model = new AdminMailForm();
            await model.Init(this.uow, id);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")]
        //TODO config datei
        public async Task<ActionResult> AdministrationMail(AdminMailForm model)
        {
            model.InitTutor(this.uow, model.ID);
            if (ModelState.IsValid)
            {
                try
                {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(model.Tutor.EMail));
                    message.From = new MailAddress("nachhilfeboerse.info@gmail.com"); //pw: 5nUtWnsz
                    message.Subject = "Nachricht vom Administrator der Nachhilfebörse.";
                    message.Body = string.Format(model.Nachricht);
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "nachhilfeboerse.info@gmail.com",
                            Password = "5nUtWnsz"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                        return RedirectToAction("Sent");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "Ein Fehler ist aufgetreten! Tipp: Haben Sie Ihre Verbindung zum Internet überprüft?");
                }
            }
            return View(model);
        }
        [Authorize(Roles ="Admin")]
        public IActionResult MailAllTutors()
        {
            return View(new MailAllTutorsModel());
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public IActionResult MailAllTutors(MailAllTutorsModel model)
        {
            var tutors = this.uow.TutorRepository.Get(t => t.Accepted);
            foreach (var item in tutors)
            {
                SendMail(item.EMail, model.Message, model.Subject);
            }
            if (ModelState.IsValid)
                return View("Sent");
            else
                return View(model);
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Statistics()
        {
            StatisticsModel model = new StatisticsModel();
            model.Init();
            return View(model);
        }
        [Authorize]
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
