using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeFeedbackWebApp.Models;
using EmployeeFeedbackWebApp.Helpers;
using EmployeeFeedbackWebApp.Classes;

namespace EmployeeFeedbackWebApp.Controllers
{
    public class EndOfYearFeedbacksController : Controller
    {
        private EmployeeFeedbackSystemEntities1 db = new EmployeeFeedbackSystemEntities1();

        // GET: EndOfYearFeedbacks
        public ActionResult Index()
        {
            var endOfYearFeedbacks = db.EndOfYearFeedbacks.Include(e => e.FeedbackAssignment);

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

            /* Creating a list of EOY feedback cycle names for listing out in the view 
             * instead of the ID values.  We'll use ViewData in Index.cshtml for this purpose
             * */
            List<FeedbackCycleViewModel> eoyCycles = new List<FeedbackCycleViewModel>();
            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "eoy").ToList().ForEach(c =>
               {
                   eoyCycles.Add(new FeedbackCycleViewModel(c.FeedbackCycleId, c.FeedbackCycleName));
               });
            ViewData["EOY"] = eoyCycles;

            return View(endOfYearFeedbacks.ToList());
        }

        // GET: EndOfYearFeedbacks/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EndOfYearFeedback endOfYearFeedback = db.EndOfYearFeedbacks.Find(id);
            if (endOfYearFeedback == null)
            {
                return HttpNotFound();
            }
            //Creating our lists to hold the EOY cycle name and the assigned user
            List<FeedbackCycleViewModel> eoyCycles = new List<FeedbackCycleViewModel>();
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
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "eoy").ToList().ForEach(c =>
            {
                eoyCycles.Add(new FeedbackCycleViewModel(c.FeedbackCycleId, c.FeedbackCycleName));
            });
            ViewData["EOY"] = eoyCycles;


            return View(endOfYearFeedback);
        }

        // GET: EndOfYearFeedbacks/Create
        public ActionResult Create()
        {
            var eoy = new EndOfYearFeedback();
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
                ViewBag.EOYFeedbackAssignmentId = new SelectList(listItem, "Text", "Value");
            });



            /* Retrieving a list of EOY feedback cycle names for listing out in the view 
           * instead of the ID values.  Using the ViewBag to assign directly to the field
           * seems to make the most sense, and doesn't require significant modifications to the view.
           * However I fully admit that this may not be optimal.
           * */
            List<SelectListItem> eoyCycles = new List<SelectListItem>();
            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "eoy").ToList().ForEach(c =>
            {
                eoyCycles.Add(new SelectListItem() { Value = c.FeedbackCycleName, Text = c.FeedbackCycleId.ToString() });
                ViewBag.EOYFeedbackCycleId = new SelectList(eoyCycles, "Text", "Value");
            });

            //Getting the max value of the EOYFeedbackCycleID so that we can iterate upon it
            //This was done because I forgot to set the Identity field in the database, so a
            //workaround was necessary
            long? maxID = db.spMaxEOYId().FirstOrDefault();
            eoy.EOYFeedbackId = (long)maxID;
            


            //ViewBag.EOYFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "UserId");
            return View(eoy);
        }

        // POST: EndOfYearFeedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EOYFeedbackId,EOYOverallPerf,EOYCommentsPositive,EOYCommentsImprove,EOYFeedbackCycleId,EOYFeedbackAssignmentId")] EndOfYearFeedback endOfYearFeedback)
        {
            if (ModelState.IsValid)
            {
                db.EndOfYearFeedbacks.Add(endOfYearFeedback);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EOYFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "FeedbackAssignmentId", endOfYearFeedback.EOYFeedbackAssignmentId);
            return View(endOfYearFeedback);
        }

        // GET: EndOfYearFeedbacks/Edit/5
        public ActionResult Edit(long? id)
        {
            //If we don't have an id, then throw an error page.
            //Otherwise we look for the user by their ID, if we do not have a match, throw another error
            //If we do have a match, we return the user like we normally would.
            //For future consideration, have this redirect instead of the error page
            //This code was provided through the scaffolding that was automatically generated
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EndOfYearFeedback endOfYearFeedback = db.EndOfYearFeedbacks.Find(id);
            if (endOfYearFeedback == null)
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
            }).Where(u => u.UserStatus == 1).Where(u=>u.RoleId >= 2).ToList().ForEach(u =>
            {
                listItem.Add(new SelectListItem() { Value = u.FullName, Text = u.UserId.ToString() });
                ViewBag.EOYFeedbackAssignmentId = new SelectList(listItem, "Text", "Value");
            });



            /* Retrieving a list of EOY feedback cycle names for listing out in the view 
           * instead of the ID values.  Using the ViewBag to assign directly to the field
           * seems to make the most sense, and doesn't require significant modifications to the view.
           * However I fully admit that this may not be optimal.
           * */
            List<SelectListItem> eoyCycles = new List<SelectListItem>();
            db.FeedbackCycles.Select(c => new
            {
                FeedbackCycleId = c.FeedBackCycleId,
                FeedbackCycleName = c.FeedbackCycleName,
                IsActive = c.IsActive,
                ReviewPeriodType = c.ReviewPeriodType
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "eoy").ToList().ForEach(c =>
            {
                eoyCycles.Add(new SelectListItem() { Value = c.FeedbackCycleName, Text = c.FeedbackCycleId.ToString() });
                ViewBag.EOYFeedbackCycleId = new SelectList(eoyCycles, "Text", "Value");
            });


            //ViewBag.EOYFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "FeedbackAssignmentId", endOfYearFeedback.EOYFeedbackAssignmentId);
            return View(endOfYearFeedback);
        }

        // POST: EndOfYearFeedbacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EOYFeedbackId,EOYOverallPerf,EOYCommentsPositive,EOYCommentsImprove,EOYFeedbackCycleId,EOYFeedbackAssignmentId")] EndOfYearFeedback endOfYearFeedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(endOfYearFeedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EOYFeedbackAssignmentId = new SelectList(db.FeedbackAssignments, "FeedbackAssignmentId", "FeedbackAssignmentId", endOfYearFeedback.EOYFeedbackAssignmentId);
            return View(endOfYearFeedback);
        }

        // GET: EndOfYearFeedbacks/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EndOfYearFeedback endOfYearFeedback = db.EndOfYearFeedbacks.Find(id);
            if (endOfYearFeedback == null)
            {
                return HttpNotFound();
            }
            //Creating our lists to hold the EOY cycle name and the assigned user
            List<FeedbackCycleViewModel> eoyCycles = new List<FeedbackCycleViewModel>();
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
            }).Where(c => c.IsActive == "Active").Where(c => c.ReviewPeriodType == "eoy").ToList().ForEach(c =>
            {
                eoyCycles.Add(new FeedbackCycleViewModel(c.FeedbackCycleId, c.FeedbackCycleName));
            });
            ViewData["EOY"] = eoyCycles;

            return View(endOfYearFeedback);
        }

        // POST: EndOfYearFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            EndOfYearFeedback endOfYearFeedback = db.EndOfYearFeedbacks.Find(id);
            db.EndOfYearFeedbacks.Remove(endOfYearFeedback);
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
