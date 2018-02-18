using Filarkiv.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Filarkiv.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload  
        public ActionResult Index()
        {
            string[] subfolders = null;
            var settings = ConfigurationManager.AppSettings.AllKeys;

            if (!settings.Contains("azure"))
            {
                string path = HttpContext.Server.MapPath("~/App_Data/Archive");
                subfolders = System.IO.Directory.GetDirectories(path);
            }

            IEnumerable<FileListViewModel> list = subfolders.ToList().Select(x => new FileListViewModel()
            {
                Files = Directory.GetFiles(x).ToList().Select(y => new Models.File()
                {
                    fileName = Path.GetFileName(y),
                    filePath = Path.GetDirectoryName(y) + "/" + Path.GetFileName(y)
                }),

                Category = x.Split('\\').Last().Substring(0,1).ToUpper() + x.Split('\\').Last().Substring(1)
            });
            
            return View(list);
        }

        [HttpGet]
        public FileResult downloadFile(string path)
        {
            string name = Path.GetFileName(path);
            return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }

        [HttpGet]
        public ActionResult UploadFiles()
        {
            var settings = ConfigurationManager.AppSettings.AllKeys;

            if (!settings.Contains("azure"))
            {
                return GetSubfolders();
            }
            return View();
        }

        private ActionResult GetSubfolders()
        {
            IEnumerable<SelectListItem> list = null;
            string path = HttpContext.Server.MapPath("~/App_Data/Archive");

            if (System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories) != null)
            {
                string[] subfolders = System.IO.Directory.GetDirectories(path);
                list = subfolders.ToList().Select(x => new SelectListItem() { Text = x.Split('\\').Last().Substring(0, 1).ToUpper() + x.Split('\\').Last().Substring(1), Value = x.Split('\\').Last() });
                var files = new Filemodel();
                files.Categories = list;

                return View(files);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] postedFiles, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)  
                    foreach (HttpPostedFileBase file in postedFiles)
                    {
                        if (file != null)
                        {
                            var Category = "";
                            var InputFileName = Path.GetFileName(file.FileName);
                            if (collection["NewCategory"].ToString().Length > 1)
                            {
                                Category = collection["NewCategory"].ToString().Trim().ToLower().Replace(' ', '-');
                                SaveFileWithNewCategory(postedFiles, file, Category, InputFileName);
                            }
                            else if (collection["SelectedCategory"].Length != 0)
                            {
                                Category = collection["SelectedCategory"];
                                SaveFile(postedFiles, file, Category, InputFileName);
                            }
                        }
                        else return View();
                    }

                return GetSubfolders();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

        private void SaveFileWithNewCategory(HttpPostedFileBase[] postedFiles, HttpPostedFileBase file, string Category, string InputFileName)
        {
            var newPath = Server.MapPath("~/App_Data/Archive/" + Category);
            Directory.CreateDirectory(newPath);
            SaveFile(postedFiles, file, Category, InputFileName);
        }

        private void SaveFile(HttpPostedFileBase[] postedFiles, HttpPostedFileBase file, string Category, string InputFileName)
        {
            var ServerSavePath = Path.Combine(Server.MapPath("~/App_Data/Archive/") + Category + "/" + InputFileName);
            file.SaveAs(ServerSavePath);
            ViewBag.UploadStatus = postedFiles.Count().ToString() + " files uploaded successfully.";
        }
    }
}