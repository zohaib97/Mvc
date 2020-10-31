using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using domain.Models;

namespace domain.Controllers
{
  
    public class usersController : Controller
    {
       
          domainEntities db = new domainEntities();

          

        // GET: users
        public ActionResult Index()
        {
            Session["count"] = db.supports.Count();
            Session["adcount"] = db.adsfiles.Count();
            Session["dcount"] = db.domain_u.Where(x => x.domain_status == "Not_Approve").Count();
            var users = db.users.Where(x => x.role_id == 2).Include(u => u.role);
            return View(users.ToList());
        }

        // GET: users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: users/Create
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(db.roles, "roleid", "rolename");
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userid,username,useremail,userpass,role_id,userphone,skype_id,domain,Daily_pages_views")] user user)
        {
            if (ModelState.IsValid)
            {
                user.status = "Active";
                db.users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.role_id = new SelectList(db.roles, "roleid", "rolename", user.role_id);
            return View(user);
        }

        // GET: users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.role_id = new SelectList(db.roles, "roleid", "rolename", user.role_id);
            return View(user);
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userid,username,useremail,userpass,role_id,userphone,skype_id,domain,Daily_pages_views")] user user)
        {
            if (ModelState.IsValid)
            {
                user.status = "Active";
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.role_id = new SelectList(db.roles, "roleid", "rolename", user.role_id);
            return View(user);
        }

        // GET: users/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index", "users");
            }
        }

        // POST: users/Delete/5
        //[HttpPost]
        //  [ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    user user = db.users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    db.users.Remove(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Adddomain()
        {
            var userid = Convert.ToInt32( Session["userid"]);
            var a = db.users.Where(x => x.userid == userid).FirstOrDefault();
            ViewBag.userid = a.userid;
            return View();
        }

        [HttpPost]
        public ActionResult Adddomain(domain_u domain_U)
        {
            db.domain_u.Add(domain_U);
            db.SaveChanges();

            return RedirectToAction("Index","Home");
        
        }

        public ActionResult Adtags()
        {


            return View();

        }
    }
}
