using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeFeedbackWebApp.Models;
using EmployeeFeedbackWebApp.Classes;

namespace EmployeeFeedbackWebApp.Controllers
{
    public class MidYearFeedbacksController : Controller
    {
        private EmployeeFeedbackSystemEntities1 db = new EmployeeFeedbackSystemEntities1();

        // GET: MidYearFeedbacks
        public ActionResult Index()
        {
            var midYearFeedbacks = db.MidYearFeedbacks.Include(m => m.FeedbackAssignment);

            /* Creating a list of active users for display in the view to show who each review
             * has been assigned feedback. We'll use ViewData in Index.cshtml for this purpose
             * */
            List<UserViewModel> lstUsers = new List<UserViewModel>();
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                UserStatus = u.UserStatus,
                FullName = u.FullName
            }).Where(u => u.UserStatus == 1).ToList().ForEach(u =>
            {
                lstUsers.Add(new UserViewModel(u.UserId, u.FullName, u.RoleId));
            });
            ViewData["MyUsers"] = lstUsers;

            /* Creating a list of Mid-year feedback cycle names for listing out in the view 
             * instead of the ID values.  We'll use ViewData in Index.cshtml for this purpose
             * */
            List<FeedbackCycleViewModel> midCycles = new List<FeedbackCycleViewModel>();

            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "midyear").ToList().ForEach(c =>
            {
                midCycles.Add(new FeedbackCycleViewModel(c.FeedbackCycleId, c.FeedbackCycleName));
            });
            ViewData["MID"] = midCycles;

                return View(midYearFeedbacks.ToList());
        }

        // GET: MidYearFeedbacks/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MidYearFeedback midYearFeedback = db.MidYearFeedbacks.Find(id);
            if (midYearFeedback == null)
            {
                return HttpNotFound();
            }
            //Creating our lists to hold the EOY cycle name and the assigned user
            List<FeedbackCycleViewModel> midCycles = new List<FeedbackCycleViewModel>();
            List<UserViewModel> lstUsers = new List<UserViewModel>();

            /* Retrieving a list of active users for display in the view to show who each review
             * has been assigned feedback.  We'll use the ViewData assignment in the Details.cshtml 
             * for this purpose.
             * */
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                UserStatus = u.UserStatus,
                FullName = u.FullName
            }).Where(u => u.UserStatus == 1).ToList().ForEach(u =>
            {
                lstUsers.Add(new UserViewModel(u.UserId, u.FullName, u.RoleId));
            });
            ViewData["MyUsers"] = lstUsers;

            /* Retrieving a list of Mid-year feedback cycle names for listing out in the view 
             * instead of the ID values.  We'll use the ViewData assignment in the Details.cshtml 
             * for this purpose.
             * */
            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "midyear").ToList().ForEach(c =>
            {
                midCycles.Add(new FeedbackCycleViewModel(c.FeedbackCycleId, c.FeedbackCycleName));
            });
            ViewData["MID"] = midCycles;

            return View(midYearFeedback);
        }

        // GET: MidYearFeedbacks/Create
        public ActionResult Create()
        {
            var mid = new MidYearFeedback();

            /* Creating the list for the Feedback Assignment field.
            * This list will contain all of the users.
            * In this version of the app, managers will report to themselves.
            * Future versions would consider ways of working around this if possible.
            * */
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem() { Value = "-- Select --", Text = "noname", Selected = true });
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName,
                RoleId = u.RoleId,
                UserStatus = u.UserStatus
            }).Where(u => u.UserStatus == 1).Where(u => u.RoleId >= 2).ToList().ForEach(u =>
            {
                listItem.Add(new SelectListItem() { Value = u.FullName, Text = u.UserId.ToString() });
                ViewBag.MidYearFeedbackAssignmentId = new SelectList(listItem, "Text", "Value");
            });

            /* Retrieving a list of mid-year feedback cycle names for listing out in the view 
           * instead of the ID values.  Using the ViewBag to assign directly to the field
           * seems to make the most sense, and doesn't require significant modifications to the view.
           * However I fully admit that this may not be optimal.
           * */
            List<SelectListItem> midCycles = new List<SelectListItem>();
            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "midyear").ToList().ForEach(c =>
            {
                midCycles.Add(new SelectListItem() { Value = c.FeedbackCycleName, Text = c.FeedbackCycleId.ToString() });
                ViewBag.MidYearFeedbackCycleId = new SelectList(midCycles, "Text", "Value");
            });

            /* Getting the max value of the MidYearFeedbackCycleId so that we can iterate
             * upon it.  This was done because I forgot to set the Identity field inthe database,
             * so a workaround was necessary */
            long? maxID = db.spMaxMidYearId().FirstOrDefault();
            mid.MidYearFeedbackId = (long)maxID;

            //ViewBag.MidYearFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "FeedbackAssignmentId");
            return View(mid);
        }

        // POST: MidYearFeedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MidYearFeedbackId,MidPerfIndicator,MidCommentsPositive,MidCommentsImprove,MidYearFeedbackCycleId,MidYearFeedbackAssignmentId")] MidYearFeedback midYearFeedback)
        {
            if (ModelState.IsValid)
            {
                db.MidYearFeedbacks.Add(midYearFeedback);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MidYearFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "FeedbackAssignmentId", midYearFeedback.MidYearFeedbackAssignmentId);
            return View(midYearFeedback);
        }

        // GET: MidYearFeedbacks/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MidYearFeedback midYearFeedback = db.MidYearFeedbacks.Find(id);
            if (midYearFeedback == null)
            {
                return HttpNotFound();
            }
            /* Creating the list for the Feedback Assignment field.
             * This list will contain all of the users.
             * In this version of the app, managers will report to themselves.
             * Future versions would consider ways of working around this if possible.
             * */
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem() { Value = "-- Select --", Text = "noname", Selected = true });
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName,
                RoleId = u.RoleId,
                UserStatus = u.UserStatus
            }).Where(u => u.UserStatus == 1).Where(u => u.RoleId >= 2).ToList().ForEach(u =>
            {
                listItem.Add(new SelectListItem() { Value = u.FullName, Text = u.UserId.ToString() });
                ViewBag.MidYearFeedbackAssignmentId = new SelectList(listItem, "Text", "Value");
            });

            /* Retrieving a list of EOY feedback cycle names for listing out in the view 
           * instead of the ID values.  Using the ViewBag to assign directly to the field
           * seems to make the most sense, and doesn't require significant modifications to the view.
           * However I fully admit that this may not be optimal.
           * */
            List<SelectListItem> midCycles = new List<SelectListItem>();
            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "midyear").ToList().ForEach(c =>
            {
                midCycles.Add(new SelectListItem() { Value = c.FeedbackCycleName, Text = c.FeedbackCycleId.ToString() });
                ViewBag.MidYearFeedbackCycleId = new SelectList(midCycles, "Text", "Value");
            });

            //ViewBag.MidYearFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "FeedbackAssignmentId", midYearFeedback.MidYearFeedbackAssignmentId);
            return View(midYearFeedback);
        }

        // POST: MidYearFeedbacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MidYearFeedbackId,MidPerfIndicator,MidCommentsPositive,MidCommentsImprove,MidYearFeedbackCycleId,MidYearFeedbackAssignmentId")] MidYearFeedback midYearFeedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(midYearFeedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MidYearFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "FeedbackAssignmentId", midYearFeedback.MidYearFeedbackAssignmentId);
            return View(midYearFeedback);
        }

        // GET: MidYearFeedbacks/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MidYearFeedback midYearFeedback = db.MidYearFeedbacks.Find(id);
            if (midYearFeedback == null)
            {
                return HttpNotFound();
            }

            //Creating our lists to hold the EOY cycle name and the assigned user
            List<FeedbackCycleViewModel> midCycles = new List<FeedbackCycleViewModel>();
            List<UserViewModel> lstUsers = new List<UserViewModel>();

            db.Users.Select(u => new
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                UserStatus = u.UserStatus,
                FullName = u.FullName
            }).Where(u => u.UserStatus == 1).ToList().ForEach(u =>
            {
                lstUsers.Add(new UserViewModel(u.UserId, u.FullName, u.RoleId));
            });
            ViewData["MyUsers"] = lstUsers;

            /* Retrieving a list of EOY feedback cycle names for listing out in the view 
             * instead of the ID values.  We'll use the ViewData assignment in the Details.cshtml 
             * for this purpose.
             * */
            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "midyear").ToList().ForEach(c =>
            {
                midCycles.Add(new FeedbackCycleViewModel(c.FeedbackCycleId, c.FeedbackCycleName));
            });
            ViewData["MID"] = midCycles;

            return View(midYearFeedback);
        }

        // POST: MidYearFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MidYearFeedback midYearFeedback = db.MidYearFeedbacks.Find(id);
            db.MidYearFeedbacks.Remove(midYearFeedback);
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
