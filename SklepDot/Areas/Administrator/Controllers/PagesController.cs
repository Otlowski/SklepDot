using SklepDot.Areas.Administrator.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SklepDot.Areas.Administrator.Models.Data;

namespace SklepDot.Areas.Administrator.Controllers
{
    public class PagesController : Controller
    {
        // GET: Administrator/Pages
        public ActionResult Index()
        {
            //Declare list of PageVM
            List<PageVM> pagesList;

            using (Db db = new Db()) {
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();

                //Init the list
            }
            return View(pagesList);
        }

        //GET : Administrator/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        //POST : Administrator/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            using (Db db = new Db())
            {
                //Declare slug
                string slug;

                PageDTO dto = new PageDTO();

                dto.Title = model.Title;

                //Init the list
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Title or slug already exists.");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();

            }

            TempData["SuccessMessage"] = "Page has been added sucessfully.";

            return RedirectToAction("AddPage");
        }

        //GET : Administrator/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("Page does not exist.");
                }

                model = new PageVM(dto);
            }
                //Declare slug
            return View(model);
        }

        //Post : Administrator/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            using (Db db = new Db())
            {
                //Get the id
                int id = model.Id;

                //Declare slug
                string slug="home";

                PageDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;

                //Init the list
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }

                }
                
                
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug)))
                {
                    ModelState.AddModelError("", "Title or slug already exists.");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                db.SaveChanges();

            }

            TempData["SuccessMessage"] = "Page has been edited sucessfully.";

            return RedirectToAction("EditPage");
        }

        //Get : Administrator/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("Page does not exist.");
                }

                model = new PageVM(dto);
            }
            //Declare slug
            return View(model);
        }

        //Get : Administrator/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id) {

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);
                db.Pages.Remove(dto);
                db.SaveChanges();
            }
            //Declare slug
            return RedirectToAction("Index");
        }
    }
}