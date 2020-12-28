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
            //create viewmodel to fill with data to pass to the view
            AvailablePosistionsViewModel viewModel = new AvailablePosistionsViewModel();
            var userId = User.Identity.GetUserId();
            var userRoles = await UserManager.GetRolesAsync(userId);
            var openPositions = db.OpenPositions.Include(o => o.Location).Include(o => o.Position);


            var stringToCheck = "Manager";

            if (userRoles.Any(stringToCheck.Contains))
            {
                //due to the fact that openPositions is an IEnumerable we change it to a list to query on to get our results
                viewModel.Positions = openPositions.ToList().Where(o => o.Location.ManagerId == userId).ToList();
                return View(viewModel.Positions);
            }
            else
            {
                viewModel.Positions = openPositions.ToList();
                return View(openPositions.ToList());
            }
        }

        // POST: Applications/CreateApplication
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public ActionResult CreateApplication(int positionId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                //makes application variable to fill in with data to pass to the applications table
                var application = new Application() {
                    ApplicationDate = DateTime.Now,
                    OpenPositionId = positionId,
                    UserId = userId,
                    ApplicationStatus = 5 //pending status
                };
                db.Applications.Add(application);
                db.SaveChanges();
            }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
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
