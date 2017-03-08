using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeFeedbackWebApp.Models;

namespace EmployeeFeedbackWebApp.Controllers
{
    public class FeedbackAssignmentsController : Controller
    {
        private EmployeeFeedbackSystemEntities1 db = new EmployeeFeedbackSystemEntities1();

        // GET: FeedbackAssignments
        public ActionResult Index()
        {
            var feedbackAssignments = db.FeedbackAssignments.Include(f => f.FeedbackCycle).Include(f => f.User);
            return View(feedbackAssignments.ToList());
        }

        // GET: FeedbackAssignments/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackAssignment feedbackAssignment = db.FeedbackAssignments.Find(id);
            if (feedbackAssignment == null)
            {
                return HttpNotFound();
            }
            return View(feedbackAssignment);
        }

        // GET: FeedbackAssignments/Create
        public ActionResult Create()
        {
            ViewBag.FeedbackCycleId = new SelectList(db.FeedbackCycles, "FeedBackCycleId", "FeedbackCycleName");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName");
            return View();
        }

        // POST: FeedbackAssignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeedbackAssignmentId,UserId,FeedbackCycleId")] FeedbackAssignment feedbackAssignment)
        {
            if (ModelState.IsValid)
            {
                db.FeedbackAssignments.Add(feedbackAssignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FeedbackCycleId = new SelectList(db.FeedbackCycles, "FeedBackCycleId", "FeedbackCycleName", feedbackAssignment.FeedbackCycleId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", feedbackAssignment.UserId);
            return View(feedbackAssignment);
        }

        // GET: FeedbackAssignments/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackAssignment feedbackAssignment = db.FeedbackAssignments.Find(id);
            if (feedbackAssignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.FeedbackCycleId = new SelectList(db.FeedbackCycles, "FeedBackCycleId", "FeedbackCycleName", feedbackAssignment.FeedbackCycleId);
            ViewBag.UserId = new SelectList(db.Users, "UserId","FullName", feedbackAssignment.UserId);
            return View(feedbackAssignment);
        }

        // POST: FeedbackAssignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeedbackAssignmentId,UserId,FeedbackCycleId")] FeedbackAssignment feedbackAssignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedbackAssignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FeedbackCycleId = new SelectList(db.FeedbackCycles, "FeedBackCycleId", "FeedbackCycleName", feedbackAssignment.FeedbackCycleId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FullName", feedbackAssignment.UserId);
            return View(feedbackAssignment);
        }

        // GET: FeedbackAssignments/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackAssignment feedbackAssignment = db.FeedbackAssignments.Find(id);
            if (feedbackAssignment == null)
            {
                return HttpNotFound();
            }
            return View(feedbackAssignment);
        }

        // POST: FeedbackAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FeedbackAssignment feedbackAssignment = db.FeedbackAssignments.Find(id);
            db.FeedbackAssignments.Remove(feedbackAssignment);
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
