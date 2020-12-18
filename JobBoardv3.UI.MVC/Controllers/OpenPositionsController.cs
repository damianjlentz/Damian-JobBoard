using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class OpenPositionsController : Controller
    {
        public OpenPositionsController()
        {

        }

        public OpenPositionsController(ApplicationUserManager userManager)
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


        // GET: OpenPositions
        public async Task<ActionResult> Index()
        {
            AvailablePosistionsViewModel viewModel = new AvailablePosistionsViewModel();
            var userId = User.Identity.GetUserId();
            var userRoles = await UserManager.GetRolesAsync(userId);
            var openPositions = db.OpenPositions.Include(o => o.Location).Include(o => o.Position);

            var stringToCheck = "Manager";

            if (userRoles.Any(stringToCheck.Contains))
            {
                viewModel.Positions = openPositions.ToList().Where(o => o.Location.ManagerId == userId).ToList();
                return View(viewModel.Positions/*, openPositionsAppliedFor.ToList()*/);
            }
            else
            {
                viewModel.Positions = openPositions.ToList();
                return View(openPositions.ToList()/*, openPositionsAppliedFor.ToList()*/);
            }

            //var openPositionsAppliedFor = from a in db.Applications
            //                   where a.UserId == userId
            //                  select a.OpenPositionId;
            //viewModel.ApplicationIds = openPositionsAppliedFor.ToList();

            //return View(openPositions.ToList()/*, openPositionsAppliedFor.ToList()*/);
        }

        //GET: Applications/CreateApplication
        //[Authorize(Roles = "Admin, Manager, Employee")]
        //public ActionResult CreateApplication(int id)
        //{
        //    Application application = new Application();
        //    OpenPosition openPosition = db.OpenPositions.Where(b => b.PositionId == id).FirstOrDefault();
        //    application.OpenPositionId = openPosition.OpenPositionId;
        //    Position position = db.Positions.Find(id);
        //    ViewBag.JobTitle = position.Title;
        //    ViewBag.UserEmail = User.Identity.GetUserName();
        //    return View(application);
        //}

        // POST: Applications/CreateApplication
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public ActionResult CreateApplication(int positionId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var application = new Application() {
                    ApplicationDate = DateTime.Now,
                    OpenPositionId = positionId,
                    UserId = userId,
                    ApplicationStatus = 5
                };
                db.Applications.Add(application);
                db.SaveChanges();
            }

            //ViewBag.ApplicationStatus = new SelectList(db.ApplicationStatus, "ApplicationStatusId", "StatusName", application.ApplicationStatus);
            //ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId", application.OpenPositionId);
            //ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName", application.UserId);
            return RedirectToAction("Index");
        }

        // GET: OpenPositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            return View(openPosition);
        }

        // GET: OpenPositions/Create
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber");
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title");
            return View();
        }

        // POST: OpenPositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manageer")]
        public ActionResult Create([Bind(Include = "OpenPositionId,PositionId,LocationId")] OpenPosition openPosition)
        {
            if (ModelState.IsValid)
            {
                db.OpenPositions.Add(openPosition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber", openPosition.LocationId);
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", openPosition.PositionId);
            return View(openPosition);
        }

        // GET: OpenPositions/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber", openPosition.LocationId);
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", openPosition.PositionId);
            return View(openPosition);
        }

        // POST: OpenPositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult Edit([Bind(Include = "OpenPositionId,PositionId,LocationId")] OpenPosition openPosition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(openPosition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber", openPosition.LocationId);
            ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", openPosition.PositionId);
            return View(openPosition);
        }

        // GET: OpenPositions/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            return View(openPosition);
        }

        // POST: OpenPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            OpenPosition openPosition = db.OpenPositions.Find(id);
            db.OpenPositions.Remove(openPosition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        ////apply functionality Get
        //ViewBag.ApplicationStatus = new SelectList(db.ApplicationStatus, "ApplicationStatusId", "StatusName");
        //ViewBag.OpenPositionId = new SelectList(db.OpenPositions, "OpenPositionId", "OpenPositionId");
        //ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "FirstName");
        //    return View();

        ////apply functionality Post
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Apply([Bind(Include = "OpenPositionId,PositionId,LocationId")] OpenPosition openPosition)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.OpenPositions.Add(openPosition);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "StoreNumber", openPosition.LocationId);
        //    ViewBag.PositionId = new SelectList(db.Positions, "PositionId", "Title", openPosition.PositionId);
        //    return View(openPosition);
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
