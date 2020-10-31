using domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace domain.Controllers
{
    public class HomeController : Controller
    {
        domainEntities db = new domainEntities();
        [Authorize(Roles ="User,Admin")]
        public ActionResult Index()
        {
          Session["count"] = db.supports.Count();
            Session["adcount"] = db.adsfiles.Count();
            Session["dcount"] = db.domain_u.Where(x => x.domain_status == "Not_Approve").Count();
            var w = db.domain_u.OrderByDescending(x=> x.domain_id).Where(x=> x.domain_status == "Not_Approve").FirstOrDefault();
            if(w != null)
            {
                Session["duser"] = w.domain_user;
                Session["dname"] = w.domain_name;
            }
            var v = db.adsfiles.OrderByDescending(x=> x.aid).FirstOrDefault();
            if (v != null)
            {
                Session["aname"] = v.ausername;
                Session["aemail"] = v.aemail;
            }
            var u = db.supports.OrderByDescending(x=>x.sid).FirstOrDefault();
            if (u != null)
            {
                Session["name"] = u.susername;
                Session["subject"] = u.ssubject;
            }
            if (Session["useremail"] == null)
            {
                return RedirectToAction("Index", "Auth");
            }
            return View();
        }
        public ActionResult Support()
        {


            return View();
        }

        [HttpPost]
        public ActionResult Support(HttpPostedFileBase sfile, support support)
        {
            String path = uploadimgfile(sfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image not Uploaded";
            }
            else
            {
                support.sfile = path;
                db.supports.Add(support);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
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

                        path = Path.Combine(Server.MapPath("~/Content/upload/"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "../Content/upload/"+Path.GetFileName(file.FileName);

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

        public ActionResult Adsupload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Adsupload(HttpPostedFileBase filename, adsfile adsfile)
        {
            String path = uploadimgfile1(filename);
            if (path.Equals("-1"))
            {
                ViewBag.error = "File not Uploaded";
            }
            else
            {
                adsfile.astatus = "Not_Approve";
                adsfile.filename = path;
                db.adsfiles.Add(adsfile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }


        [HttpPost]
        public string uploadimgfile1(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".csv") || extension.ToLower().Equals(".xlsx") || extension.ToLower().Equals(".docx"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload/adsupload/"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "../Content/upload/adsupload/" + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only csv ,xlsx or docs formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }

        public ActionResult Adslist()
        {
            var adlist = db.adsfiles.Where(x => x.aid != 0).ToList();
            return View(adlist);
        }


        public ActionResult AdsApprove(int id)
        {

            var a = db.adsfiles.Where(x => x.aid == id).FirstOrDefault();
            a.astatus = "Approve";
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Adslist");
        }

        public ActionResult AdsNotApprove(int id)
        {

            var a = db.adsfiles.Where(x => x.aid == id).FirstOrDefault();
            a.astatus = "Not_Approve";
            db.Entry(a).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Adslist");
        }


    }
}