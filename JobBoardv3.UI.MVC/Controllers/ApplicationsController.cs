using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobBoardv3.DATA.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace JobBoardv3.UI.MVC.Controllers
{
    
    public class ApplicationsController : Controller
    {
        private JobBoardEntities db = new JobBoardEntities();

        // GET: Applications
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Index()
        {
            var applications = db.Applications.Include(a => a.ApplicationStatu).Include(a => a.OpenPosition);
            return View(applications.ToList());
        }

        // GET: Applications/Details/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Details(int? id)
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

        //// GET: Applications/Create
        //public ActionResult Create()
        //{
        //    ViewBag.ApplicationStatus = new SelectList(db.ApplicationStatus, "ApplicationStatusId", "StatusName");
        //    ViewBag.OpenPosition = new SelectList(db.OpenPositions, "OpenPosition", "OpenPosition");
        //    //ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName");
        //    return View();
        //}

        //// POST: Applications/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ApplicationId,OpenPositionId,UserId,ApplicationDate,ManagerNotes,ApplicationStatus,ResumeFileName,FirstName,LastName")] Application application)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Applications.Add(application);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ApplicationStatus = new SelectList(db.ApplicationStatus, "ApplicationStatusId", "StatusName", application.ApplicationStatus);
        //    ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
        //    //ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", application.UserId);
        //    return View(application);
        //}

        // GET: Applications/CreateApplication
        [Authorize(Roles = "Admin, Manager, Employee")]
        public ActionResult CreateApplication(int id)
        {
            Application application = new Application();
            application.OpenPositionId = id;
            Position position = db.Positions.Find(id);
            ViewBag.JobTitle = position.Title;
            ViewBag.UserEmail = User.Identity.GetUserName();
            return View(application);
        }

        // POST: Applications/CreateApplication
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public ActionResult CreateApplication([Bind(Include = "ApplicationId,OpenPositionId,UserId,ApplicationDate,ManagerNotes,ApplicationStatus,FirstName,LastName, ResumeFile")] Application application)
        {
            if (ModelState.IsValid)
            {
                //Get Upload path from Web.Config file AppSettings.  
                string UploadPath = ConfigurationManager.AppSettings["UserResumePath"].ToString();

                //Its Create complete path to store in server.  
                application.ResumeFileName = UploadPath + application.ResumeFile.FileName;

                //To copy and save file into server.  
                application.ResumeFile.SaveAs(Server.MapPath(Path.Combine(UploadPath, application.ResumeFile.FileName)));

                var date =  DateTime.Now;
                application.ApplicationDate = date;
                application.ApplicationStatus = 1;
                db.Applications.Add(application);
                db.SaveChanges();
                return View("Confirmation");
            }

            ViewBag.ApplicationStatus = new SelectList(db.ApplicationStatus, "ApplicationStatusId", "StatusName", application.ApplicationStatus);
            ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
            //ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", application.UserId);
            return View(application);
        }

        //[HttpPost]
        //public ActionResult UploadResume(HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        if (file.ContentLength > 0)
        //        {
        //            string _FileName = Path.GetFileName(file.FileName);
        //            string _path = Path.Combine(Server.MapPath("~/UploadedResumes"), _FileName);
        //            file.SaveAs(_path);
        //        }
        //        ViewBag.Message = "File Uploaded Successfully!!";
        //        return View();
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "File upload failed!!";
        //        return View();
        //    }
        //}

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
            ViewBag.ResumeFileName = application.ResumeFileName;
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
