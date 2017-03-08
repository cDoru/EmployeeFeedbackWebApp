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
    public class UsersController : Controller
    {
        private EmployeeFeedbackSystemEntities1 db = new EmployeeFeedbackSystemEntities1();

        // GET: Users
        public ActionResult Index()
        {
            //Storing the names of the roles in the users var for display in the view later
            var users = db.Users.Include(u => u.Role);

            /* Creating a new list of managers based off the custom UserViewModel class to
             * store three pieces of user information based on those in managerial roles.
             * We'll use this list for displaying who is a manager of whom in the 
             * Reports To column later in the view
             * */
            List<UserViewModel> mgrList = new List<UserViewModel>();
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                FullName = u.FullName
                
            }).Where(u => u.RoleId == 2).ToList().ForEach(u =>
            {
                mgrList.Add(new UserViewModel(u.UserId, u.FullName, u.RoleId));
            });
            
            //Assigning the manager list a ViewData property so we can access it in the view later
            ViewData["MyData"] = mgrList;

            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(long? id)
        {
            //If we don't have an id, then throw an error page.
            //Otherwise we look for the user by their ID, if we do not have a match, throw another error
            //If we do have a match, we return the user like we normally would.
            //For future consideration, have this redirect instead of the error page
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    /* Creating a new list of managers based off the custom UserViewModel class to
                     * store three pieces of user information based on those in managerial roles.
                     * We'll use this list for displaying who is a manager of whom in the 
                     * Reports To column later in the view
                     * */
                    List<UserViewModel> mgrList = new List<UserViewModel>();
                    db.Users.Select(u => new
                    {
                        UserId = u.UserId,
                        RoleId = u.RoleId,
                        FullName = u.FullName

                    }).Where(u => u.UserId == user.ReportsTo).ToList().ForEach(u =>
                    {
                        mgrList.Add(new UserViewModel(u.UserId, u.FullName, u.RoleId));
                    });

                    ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);

                    ViewData["MyData"] = mgrList;

                    return View(user);
                }
            }
            
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            /* We are getting the current maximum userID from the Users table
             * This was done because I unfortunately did not realize that I needed
             * to auto-increment the column through the IDENTITY attribute at the time the database was
             * being put together.  Research suggests that this would require a complete drop and re-creationg of the
             * table, and in the interest of time, I implemented this stored procedure instead.
             * By no means do I think this is an efficient way of doing things, but for now it is functional.
             * */
            var usr = new User();
            long? maxUserId = db.spMaxUserId().FirstOrDefault();
            usr.UserId = (long)maxUserId;

            //Assigning the Roles to a SelectList so they can be displayed as a drop down in the view
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName");

            /* Creating the list for the Reports To field.
             * This list will contain all of the managers who employees can report to.
             * In this version of the app, managers will report to themselves.
             * Future versions would consider ways of working around this if possible.
             * */
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add( new SelectListItem() {Value= "-- Select --", Text = "noname", Selected = true });
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName,
                RoleId = u.RoleId
            }).Where(u => u.RoleId == 2).ToList().ForEach(u =>
            {
                listItem.Add(new SelectListItem() { Value = u.FullName, Text = u.UserId.ToString() });
                //ViewBag.DropDownValues = new SelectList(listItem, "Text", "Value");
                ViewBag.ReportsTo = new SelectList(listItem, "Text", "Value");
            });

            return View(usr);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Tenure,RoleId,UserStatus,EmailAddress,password,ReportsTo,FullName")] User user)
        {
            /* Checking to see if our Model that was brought back from the view is valid, and if so
             * we're going to trim extra white spaces off of the fields before adding the new record to the
             * database to ensure data integrity.  Otherwise we kick it back again.
             * */
            if (ModelState.IsValid)
            {
                user.FirstName = user.FirstName.Trim();
                user.LastName = user.LastName.Trim();
                user.EmailAddress = user.EmailAddress.Trim();
                user.FullName = user.FirstName.ToString() + " " + user.LastName.ToString();
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(long? id)
        {
            //If we don't have an id, then throw an error page.
            //Otherwise we look for the user by their ID, if we do not have a match, throw another error
            //If we do have a match, we return the user like we normally would.
            //For future consideration, have this redirect instead of the error page
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            /* Creating the list for the Reports To field.
             * This list will contain all of the managers who employees can report to.
             * In this version of the app, managers will report to themselves.
             * Future versions would consider ways of working around this if possible.
             * */
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem() { Value = "-- Select --", Text = "noname", Selected = true });
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName,
                RoleId = u.RoleId
            }).Where(u => u.RoleId == 2).ToList().ForEach(u =>
            {
                listItem.Add(new SelectListItem() { Value = u.FullName, Text = u.UserId.ToString() });
                ViewBag.ReportsTo = new SelectList(listItem, "Text", "Value");
            });

            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,Tenure,RoleId,UserStatus,EmailAddress,password,ReportsTo,FullName")] User user)
        {
            /* Checking to see if our Model that was brought back from the view is valid, and if so
             * we're going to trim extra white spaces off of the fields before adding the new record to the
             * database to ensure data integrity.  Otherwise we kick it back again.
             * */
            if (ModelState.IsValid)
            {
                user.FirstName = user.FirstName.Trim();
                user.LastName = user.LastName.Trim();
                user.EmailAddress = user.EmailAddress.Trim();
                user.FullName = user.FirstName.ToString() + " " + user.LastName.ToString();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            /* Creating a new list of managers based off the custom UserViewModel class to
             * store three pieces of user information based on those in managerial roles.
             * We'll use this list for displaying who is a manager of whom in the 
             * Reports To column later in the view
             * */
            List<UserViewModel> mgrList = new List<UserViewModel>();
            db.Users.Select(u => new
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                FullName = u.FullName

            }).Where(u => u.UserId == user.ReportsTo).ToList().ForEach(u =>
            {
                mgrList.Add(new UserViewModel(u.UserId, u.FullName, u.RoleId));
            });

            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", user.RoleId);

            ViewData["MyData"] = mgrList;

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
