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
  
    public class usersController : Controller
    {
       
          domainEntities db = new domainEntities();

          

        // GET: users
        public ActionResult Index()
        {
            var users = db.users.Where(x => x.role_id == 2).Include(u => u.role);
            return View(users.ToList());
        }
        public ActionResult filepermAllow(int id)
        {

            var a = db.users.Where(x => x.userid == id).FirstOrDefault();
            a.fileperm = "Allow";
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult filepermNotAllow(int id)
        {

            var a = db.users.Where(x => x.userid == id).FirstOrDefault();
            a.fileperm = "Not_Allow";
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
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
            
            int user = Convert.ToInt32(Session["userid"]);
           ViewBag.domain = db.domain_u.Where(x => x.domain_user == user).ToList();
            var a = db.users.Where(x => x.fileperm == "Allow" && x.userid == user).FirstOrDefault();
            if (a != null)
            {
               
                return View();
            }
            else
            {
                Session["Permission"] = "You are not Allowed to Add tags ask Admin to Allow you";
                return RedirectToAction("Index", "Home");
            }


            

        }
        [HttpPost]
        public ActionResult Adtags(usertag usertag,tag tag,string userid,string username, string[] hjdsj,int domainid)
        {
        
            tag.userid = userid;
            tag.username = username;
            tag.domainid = domainid;
            db.tags.Add(tag);
            db.SaveChanges();
            var uid = Convert.ToInt32(userid);
          //  var a = db.admincsvtags.Where(x => x.userid == uid).ToList();
            
         

            string ad = string.Join("",hjdsj);
            string[] df = ad.Split(',');
           
                foreach (var item in df)
            {
                var a = db.admincsvtags.Where(x => x.userid == uid && x.tags == item).FirstOrDefault();

                if (a != null)
                    {

                        usertag.tid = tag.tid;
                        usertag.tags = item;
                    usertag.did = domainid;
                        db.usertags.Add(usertag);
                        db.SaveChanges();
                    }
                    else
                    {
                        Session["tagerror"] = "Some of your tags are not matched with admintags";
                        return RedirectToAction("Index", "Home");
                    }
                
                    
                
                
            }

            
            return RedirectToAction("Index","Home");

        }

        public ActionResult usertags()
        {
            var list = db.tags.Where(x => x.tid != 0).ToList();
            return View(list);
        }
        public ActionResult domainlist(int? id,int doid)
        {
            var domain = db.usertags.Where(x => x.tid == id && x.did == doid).ToList();
            ViewBag.domainname = db.domain_u.Where(x => x.domain_id == doid).ToList();
            ViewBag.domainid = doid;
            return View(domain);
        }

        public ActionResult tagslist(int? id)
        {
            var tagslist = db.usertags.Where(x => x.did == id).ToList();
            return View(tagslist);
        }

        //public ActionResult tagsapprove(int id)
        //{

        //    var a = db.usertags.Where(x => x.uid == id).FirstOrDefault();
        //    a.ustatus = "Approve";
        //    db.Entry(a).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("tagslist", new { id = a.tid });
        //}

        //public ActionResult tagsnotapprove(int id)
        //{

        //    var a = db.usertags.Where(x => x.uid == id).FirstOrDefault();
        //    a.ustatus = "Not_Approve";
        //    db.Entry(a).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("tagslist", new {id=a.tid });
        //}

        public ActionResult admintagslist()
        {
            int user = Convert.ToInt32( Session["userid"]);
            var a = db.users.Where(x => x.fileperm == "Allow" && x.userid == user).FirstOrDefault();
            if (a !=null)
            {
                var list = db.admintags.Where(x => x.id != 0).ToList();
                return View(list);
            }
            else
            {
                Session["Permission"] = "You are not Allowed to see ask Admin to Allow you";
                return RedirectToAction("Index","Home");
            }
        }
        public ActionResult admintxtlist()
        {
            var txt = db.adstxts.Where(x => x.id != 0).ToList();

            return View(txt);
        }


        //public ActionResult copyadmintagslist()
        //{


        //    return View();
        //}

        //[HttpPost]
        //public ActionResult copyadmintagslist(HttpPostedFileBase postedFile,csvtag csvtag)
        //{
          
        //    string filePath = string.Empty;
        //    if (postedFile != null)
        //    {
        //        string path = Server.MapPath("~/Content/upload/adminupload/adtags/");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        filePath = path + Path.GetFileName(postedFile.FileName);
        //        string extension = Path.GetExtension(postedFile.FileName);
        //        postedFile.SaveAs(filePath);

        //        //Read the contents of CSV file.
        //        string csvData = System.IO.File.ReadAllText(filePath);

        //        //Execute a loop over the rows.
        //        foreach (string row in csvData.Split('\n'))
        //        {
        //            if (!string.IsNullOrEmpty(row))
        //            {
        //                int data = row.Split(',').Length;
        //                for (var i = 0; i <data ; i++)
        //                {
        //                    csvtag.tags = row.Split(',')[i];
        //                    //csvtag.Name = row.Split(',')[1];
        //                    //csvtag.Country = row.Split(',')[2];
        //                    db.csvtags.Add(csvtag);
        //                    db.SaveChanges();
        //                }
                        
        //            }
        //        }
        //    }

        //    return View(csvtag);
        //}
    }

    }

