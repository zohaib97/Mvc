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
    public class domain_uController : Controller
    {
        private domainEntities db = new domainEntities();

        // GET: domain_u
        public ActionResult Index()
        {
            var domain_u = db.domain_u.Include(d => d.user).ToList();
            return View(domain_u);
        }

        // GET: domain_u/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            domain_u domain_u = db.domain_u.Find(id);
            if (domain_u == null)
            {
                return HttpNotFound();
            }
            return View(domain_u);
        }

        // GET: domain_u/Create
        public ActionResult Create()
        {
            ViewBag.domain_user = db.users.Where(x => x.role_id == 2).ToList();
            return View();
        }

        // POST: domain_u/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "domain_id,domain_name,domain_user")] domain_u domain_u)
        {
            if (ModelState.IsValid)
            {
                db.domain_u.Add(domain_u);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.domain_user = db.users.Where(x => x.role_id == 2).ToList();
            return View(domain_u);
        }

        // GET: domain_u/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            domain_u domain_u = db.domain_u.Find(id);
            if (domain_u == null)
            {
                return HttpNotFound();
            }
            ViewBag.domain_user = db.users.Where(x => x.role_id == 2).ToList();
            return View(domain_u);
        }

        // POST: domain_u/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "domain_id,domain_name,domain_user")] domain_u domain_u)
        {
            if (ModelState.IsValid)
            {
                db.Entry(domain_u).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.domain_user = db.users.Where(x => x.role_id == 2).ToList();
            return View(domain_u);
        }

        // GET: domain_u/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            domain_u domain_u = db.domain_u.Find(id);
            if (domain_u == null)
            {
                return HttpNotFound();
            }
            db.domain_u.Remove(domain_u);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: domain_u/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    domain_u domain_u = db.domain_u.Find(id);
        //    db.domain_u.Remove(domain_u);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        public ActionResult DomainApprove(int id)
        {

            var a = db.domain_u.Where(x => x.domain_id == id).FirstOrDefault();
            a.domain_status = "Approve";
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DomainNotApprove(int id)
        {

            var a = db.domain_u.Where(x => x.domain_id == id).FirstOrDefault();
            a.domain_status = "Not_Approve";
            db.Entry(a).State = EntityState.Modified;
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
