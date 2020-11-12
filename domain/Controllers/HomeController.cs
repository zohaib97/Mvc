using domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
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


        public ActionResult Adstags()
        {

            ViewBag.users = db.users.Where(x=>x.role_id== 2).ToList();
            return View();

        }

        [HttpPost]
        public ActionResult Adstags(HttpPostedFileBase atag, int userid, admincsvtag admincsvtag, admintag admintag)
        {
            string filePath = string.Empty;
            if (atag != null)
            {
                string path = Server.MapPath("~/Content/upload/adminupload/adtags/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(atag.FileName);
                string extension = Path.GetExtension(atag.FileName);
                atag.SaveAs(filePath);

                admintag.atag = "../Content/upload/adminupload/adtags/" + Path.GetFileName(atag.FileName);
                admintag.userid = userid;
                db.admintags.Add(admintag);
                db.SaveChanges();
                
                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);

                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        int data = row.Split(',').Length;
                        for (var i = 0; i < data; i++)
                        {
                            admincsvtag.tags = row.Split(',')[i];
                            admincsvtag.userid = userid;
                            //csvtag.Name = row.Split(',')[1];
                            //csvtag.Country = row.Split(',')[2];
                            db.admincsvtags.Add(admincsvtag);
                            db.SaveChanges();
                        }

                    }
                }
            }

            return View("Index");

        }



        [HttpPost]
        public string uploadimgfile2(HttpPostedFileBase file)
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

                        path = Path.Combine(Server.MapPath("~/Content/upload/adminupload/adtags/"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "../Content/upload/adminupload/adtags/" + Path.GetFileName(file.FileName);

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

        public ActionResult Adstxt()
        {


            return View();

        }

        [HttpPost]
        public ActionResult Adstxt(HttpPostedFileBase txtfile, adstxt adstxt)
        {
            String path = uploadimgfile3(txtfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "File not Uploaded";
            }
            else
            {

                adstxt.txtfile = path;
                db.adstxts.Add(adstxt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();

        }



        [HttpPost]
        public string uploadimgfile3(HttpPostedFileBase file)
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

                        path = Path.Combine(Server.MapPath("~/Content/upload/adminupload/adtxt/"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "../Content/upload/adminupload/adtxt/" + Path.GetFileName(file.FileName);

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

        public ActionResult Abc()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Abc(HttpPostedFileBase file)
        {
            string filePath = string.Empty;
            if (file != null)
            {
                string path = Server.MapPath("~/Content/upload/adminupload/Revenue_File/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                file.SaveAs(filePath);

                string conString = string.Empty;

                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                conString = @"Server=.;Database=domain;user id=sa;password=123;Trusted_Connection=True;";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Revenue";

                        // Map the Excel columns with that of the database table, this is optional but good if you do
                        // 
                        sqlBulkCopy.ColumnMappings.Add("Id", "Id");
                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("Class", "Class");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            //if the code reach here means everthing goes fine and excel data is imported into database
            ViewBag.Success = "File Imported and excel data saved into database";
            return View();
        }

    }
}