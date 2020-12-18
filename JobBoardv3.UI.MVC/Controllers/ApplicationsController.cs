using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using JobBoardv3.DATA.EF;
using JobBoardv3.UI.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace JobBoardv3.UI.MVC.Controllers
{
    
    public class ApplicationsController : Controller
    {
        public ApplicationsController()
        {

        }

        public ApplicationsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private JobBoardEntities1 db = new JobBoardEntities1();

        // GET: Applications
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult> Index()
        {
            ApplicationsViewModel viewModel = new ApplicationsViewModel();
            var userId = User.Identity.GetUserId();
            var userRoles = await UserManager.GetRolesAsync(userId);
            var applications = db.Applications.Include(o => o.ApplicationStatu).Include(o => o.OpenPosition);

            var stringToCheck = "Manager";

            if (userRoles.Any(stringToCheck.Contains))
            {
                viewModel.Applications = applications.ToList().Where(o => o.OpenPosition.Location.ManagerId == userId).ToList();
                return View(viewModel.Applications);
            }
            else
            {
                viewModel.Applications = applications.ToList();
                return View(applications.ToList());
            }
        }

        // GET: Applications/Details/5
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            var user = await UserManager.FindByIdAsync(application.UserId);
            if (application == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserEmail = user.Email;
            return View(application);
        }

        

        // GET: Applications/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ResumeFileName = application.ResumeFileName;
            ViewBag.ApplicationStatus = new SelectList(db.ApplicationStatus, "ApplicationStatusId", "StatusName", application.ApplicationStatus);
            ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
            ViewBag.UserEmail = User.Identity.GetUserName();
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "ApplicationId,OpenPositionId,UserId,ApplicationDate,ManagerNotes,ApplicationStatus,ResumeFileName")] Application application)
        {
            if (ModelState.IsValid)
            {
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationStatus = new SelectList(db.ApplicationStatus, "ApplicationStatusId", "StatusName", application.ApplicationStatus);
            ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
            //ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", application.UserId);
            return View(application);
        }

        // GET: Applications/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Applications.Find(id);
            db.Applications.Remove(application);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        ////Get: Applications/Confirmation
        //public ActionResult Confirmation
        //{
        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
