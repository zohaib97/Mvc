using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using domain.Models;

namespace domain.Controllers
{
    
    public class userprofilesController : Controller
    {
        domainEntities db = new domainEntities();

        // GET: userprofiles
        public ActionResult Index()
        {
            var userprofiles = db.userprofiles.Include(u => u.user);
            return View(userprofiles.ToList());
        }

        // GET: userprofiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userprofile userprofile = db.userprofiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }

        // GET: userprofiles/Create
        [Authorize(Roles = "User")]
        public ActionResult Create()
        {
            int userid = Convert.ToInt32(Session["userid"]);
            var us = db.users.Where(x => x.userid == userid).FirstOrDefault();
            ViewBag.user_id = us.userid;
            ViewBag.user_name = us.username;
            ViewBag.user_email = us.useremail;
            ViewBag.user_pass = us.userpass;
            return View();
        }

        // POST: userprofiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "proid,proname,proemail,proimage,propass,users_id")] userprofile userprofile,HttpPostedFileBase proimage)
        {
           
                var ut = db.users.Where(o => o.userid == userprofile.users_id).FirstOrDefault();
                var ud = db.userprofiles.Where(x => x.users_id == userprofile.users_id).FirstOrDefault();
                if(ud != null)
                {
                    ut.username = userprofile.proname;
                    ut.useremail = userprofile.proemail;
                    ut.userpass = userprofile.propass;
                    db.Entry(ut).State = EntityState.Modified;
                    db.SaveChanges();
                    ud.proname = userprofile.proname;
                    ud.proemail = userprofile.proemail;
                    ud.propass = userprofile.propass;
                    if (proimage != null) {
                        string path = uploadimgfile(proimage);
                        if (path.Equals("-1"))
                        {
                            ViewBag.error = "Image could not be uploaded....";
                        }
                        else
                        {
                            ud.proimage = path;
                        }
                    }
                    else
                    {
                        ud.proimage = ud.proimage;
                    }
                    db.Entry(ud).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["username"] = ud.proname;
                    Session["userimage"] = ud.proimage;
                Session["userproid"] = ud.proid;
                return RedirectToAction("Details", new { id = ud.proid });
                }
                else
                {
                    ut.username = userprofile.proname;
                    ut.useremail = userprofile.proemail;
                    ut.userpass = userprofile.propass;
                    db.Entry(ut).State = EntityState.Modified;
                    db.SaveChanges();
                    string path = uploadimgfile(proimage);
                    if (path.Equals("-1"))
                    {
                        ViewBag.error = "Image could not be uploaded....";
                    }
                    else
                    {
                        userprofile.proimage = path;
                        db.userprofiles.Add(userprofile);
                        db.SaveChanges();
                        Session["username"] = userprofile.proname;
                        Session["userimage"] = userprofile.proimage;
                    Session["userproid"] = userprofile.proid;
                       
                    }
                    return RedirectToAction("Details", new { id = userprofile.proid });
                }


            //ViewBag.users_id = new SelectList(db.users, "userid", "username", userprofile.users_id);
          
        }

        [HttpPost]
        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/users/"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = file.FileName;

                        //    ViewBag.Message = "File uploaded successfully";
                    }

                    catch (Exception)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }
        // GET: userprofiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userprofile userprofile = db.userprofiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            ViewBag.users_id = new SelectList(db.users, "userid", "username", userprofile.users_id);
            return View(userprofile);
        }

        // POST: userprofiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "proid,proname,proemail,proimage,propass,users_id")] userprofile userprofile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userprofile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.users_id = new SelectList(db.users, "userid", "username", userprofile.users_id);
            return View(userprofile);
        }

        // GET: userprofiles/Delete/5
        [Authorize(Roles = "User")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userprofile userprofile = db.userprofiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }

        // POST: userprofiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            userprofile userprofile = db.userprofiles.Find(id);
            db.userprofiles.Remove(userprofile);
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
