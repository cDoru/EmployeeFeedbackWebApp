using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using EmployeeFeedbackWebApp.Models;
using EmployeeFeedbackWebApp.Classes;

namespace EmployeeFeedbackWebApp.Controllers
{
    public class FeedbackCyclesController : Controller
    {
        private EmployeeFeedbackSystemEntities1 db = new EmployeeFeedbackSystemEntities1();

        // GET: FeedbackCycles
        public ActionResult Index()
        {
            return View(db.FeedbackCycles.ToList());
        }

        // GET: FeedbackCycles/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackCycle feedbackCycle = db.FeedbackCycles.Find(id);
            if (feedbackCycle == null)
            {
                return HttpNotFound();
            }
            return View(feedbackCycle);
        }

        // GET: FeedbackCycles/Create
        public ActionResult Create()
        {
            var fbCycle = new FeedbackCycle();
            long? maxFbId = db.spMaxFeedBackCycleId().FirstOrDefault();
            fbCycle.FeedBackCycleId = (long)maxFbId;
            return View(fbCycle);
        }

        // POST: FeedbackCycles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeedBackCycleId,FeedbackCycleName,MidYearFeedbackCycleId,EOYFeedbackCycleId,IsActive,ReviewPeriodType")] FeedbackCycle feedbackCycle)
        {
            if (ModelState.IsValid)
            {
                db.FeedbackCycles.Add(feedbackCycle);
                db.SaveChanges();
                
                /* Based on the value the user selected for Review Period Type, we are running the appropriate stored procedure to ensure
                 * that the proper field in FeedbackCyles (EOYFeedbackCycleId or MidYearFeedbackCycleId) is updated to reflect the
                 * correct foreign key value based on the primary key of the newly added row */                 
                if(feedbackCycle.ReviewPeriodType == "eoy")
                {
                    db.spEndOfYearReviewSetFeedbackCycles();
                }
                else if(feedbackCycle.ReviewPeriodType == "midyear")
                {
                    db.spMidYearReviewSetFeedbackCycles();
                }

                return RedirectToAction("Index");
            }

            return View(feedbackCycle);
        }

        // GET: FeedbackCycles/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackCycle feedbackCycle = db.FeedbackCycles.Find(id);
            if (feedbackCycle == null)
            {
                return HttpNotFound();
            }
            return View(feedbackCycle);
        }

        // POST: FeedbackCycles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeedBackCycleId,FeedbackCycleName,MidYearFeedbackCycleId,EOYFeedbackCycleId,IsActive,ReviewPeriodType")] FeedbackCycle feedbackCycle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedbackCycle).State = EntityState.Modified;
                db.SaveChanges();
                //TODO: Add logic for UPDATING the EOYFeedbackCycleID / MidYearFeedbackCycleId fields.  This will require new stored procedures
                if(feedbackCycle.ReviewPeriodType == "eoy")
                {
                    db.spEndofYearReviewUpdateFeedbackCycles((int)feedbackCycle.FeedBackCycleId);
                }
                else if (feedbackCycle.ReviewPeriodType == "midyear")
                {
                    db.spMidYearReviewUpdateFeedbackCycles((int)feedbackCycle.FeedBackCycleId);
                }
                return RedirectToAction("Index");
            }
            return View(feedbackCycle);
        }

        // GET: FeedbackCycles/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackCycle feedbackCycle = db.FeedbackCycles.Find(id);
            if (feedbackCycle == null)
            {
                return HttpNotFound();
            }
            return View(feedbackCycle);
        }

        // POST: FeedbackCycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FeedbackCycle feedbackCycle = db.FeedbackCycles.Find(id);
            db.FeedbackCycles.Remove(feedbackCycle);
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
