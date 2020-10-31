using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using domain.Models;

namespace domain.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        // GET: Auth
        domainEntities db = new domainEntities(); 
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult loginf(string useremail,string userpass)
        {
            var us = db.users.Where(x => x.useremail == useremail && x.userpass == userpass).FirstOrDefault();

            if(us != null)
            {
                FormsAuthentication.SetAuthCookie(us.username, false);
                if(us.role_id == 1)
                {
                    Session["userid"] = us.userid;
                    Session["username"] = us.username;
                    Session["useremail"] = us.useremail;
                    Session["userrole"] = us.role_id;
                    return RedirectToAction("Index", "Home");

                }
                if(us.role_id == 2)
                {
                    if (us.status == "Active" && us.ecode != "")
                    {


                        Session["userid"] = us.userid;
                        Session["username"] = us.username;
                        Session["useremail"] = us.useremail;
                        Session["userrole"] = us.role_id;
                        var uss = db.userprofiles.Where(x => x.users_id == us.userid).FirstOrDefault();
                        if(uss != null)
                        {
                            Session["userproid"] = uss.proid;
                            Session["userimage"] = uss.proimage;
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Auth");
                    }
                    }

            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
            return View();
        }
        public ActionResult signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(string username, string useremail, string userpass, string userphone, string skype_id, string domain, string daily_page, user user)
        {
            Random r = new Random();
            user.username = username;
            user.useremail = useremail;
            user.userpass = userpass;
            user.userphone = userphone;
            user.skype_id = skype_id;
            user.role_id = 2;
            user.domain = domain;
            user.Daily_pages_views = daily_page;
            user.status = "NotActive";
            var activationCode = r.Next();
            user.ecode = Convert.ToString(activationCode);
             db.users.Add(user);
            db.SaveChanges();
            var GenarateUserVerificationLink = "/Auth/UserVerification?id=" + activationCode+"&e="+user.userid;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            string from = "ibrahimnawab4567@gmail.com";

            string pass = "Dawood987";
            string host = "smtp.gmail.com";
            string port = "587";

            MailMessage mail = new MailMessage();
            mail.To.Add(user.useremail);
            mail.From = new MailAddress(from);
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            mail.Subject = "Registration Completed-Demo";
            mail.Body = "<br/> Your registration completed succesfully." +
                           "<br/> please click on the below link for account verification" +
                           "<br/><br/><a href=" + link + ">" + link + "</a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = host;
            smtp.Port = int.Parse(port);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(from, pass);
            smtp.EnableSsl = true;
           
            smtp.Send(mail);

            Session["sms"] = "register";
            return RedirectToAction("Index", "Auth");
        }
        public ActionResult UserVerification(string id,int e)
        {
            var us = db.users.Where(x => x.ecode == id && x.userid == e).FirstOrDefault();

            if(us != null)
            {   
                us.status = "Active";
                db.Entry(us).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                Session["sms"] = "register success";
                return RedirectToAction("Index", "Auth");
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
        }

        public ActionResult logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Auth");
        }
        //public void mailing()
        //{
        //    string from = System.Configuration.ConfigurationManager.AppSettings["mail_sender"];
        //    string pass = System.Configuration.ConfigurationManager.AppSettings["mail_sender_pass"];
        //    string host = System.Configuration.ConfigurationManager.AppSettings["mail_host"];
        //    string port = System.Configuration.ConfigurationManager.AppSettings["mail_port"];

        //    MailMessage mail = new MailMessage();
        //    mail.To.Add(from);
        //    mail.From = new MailAddress(from);
        //    mail.Subject = "Countax Services ~ Data upload info";
        //    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
        //    SmtpClient smtp = new SmtpClient();
        //    smtp.Host = host;
        //    smtp.Port = int.Parse(port);
        //    smtp.UseDefaultCredentials = true;
        //    smtp.Credentials = new System.Net.NetworkCredential(from, pass);
        //    smtp.EnableSsl = true;

        //    smtp.Send(mail);

        //}
    }
}