using SklepDot.Areas.Administrator.Models.Data;
using SklepDot.Areas.Administrator.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SklepDot.Areas.Administrator.Controllers
{
    public class ShopController : Controller
    {
        // GET: Administrator/Shop
        public ActionResult Categories()
        {
            List<CategoryVM> categoryVMlist;

            using (Db db = new Db())
            {
                //Init the list
                categoryVMlist = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            return View(categoryVMlist);
        }

        //POST : Administrator/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (Db db = new Db())
            {

                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                CategoryDTO dto = new CategoryDTO();

                dto.Name = catName;
                dto.Desc = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();
         
            }

            return id;
        }

        //POST : Administrator/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            

            using (Db db = new Db())
            {
                int count = 1;

                CategoryDTO dto;

                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();

                    count++;
                }
            }   
        }

        //Get : Administrator/Shop/DeleteCategory
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {

            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);
                db.Categories.Remove(dto);
                db.SaveChanges();
            }
            //Declare slug
            return RedirectToAction("Categories");
        }

        //POST : Administrator/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {

                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = newCatName;
                dto.Desc = newCatName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();

            }

            return "success";
        }

        [HttpGet]
        //Get : Administrator/Shop/AddProduct
        public ActionResult AddProduct(string proName)
        {
            ProductVM model = new ProductVM();

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                
            }
            return View(model);
        }

        [HttpPost]
        //POST : Administrator/Shop/AddProduct
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                }
            }

            using (Db db = new Db())
            {

                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Name already exists.");
                    return View(model);
                }
            }

            int id;
            using (Db db = new Db())
            {

                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                model.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;


            }
            TempData["SuccessMessage"] = "Product has been added.";

            #region Upload Image

            var originalDirevtory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(originalDirevtory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirevtory.ToString(), "Products" + id.ToString());
            var pathString3 = Path.Combine(originalDirevtory.ToString(), "Products" + id.ToString() + "\\Thumbs");


            if (!Directory.Exists(pathString1))
                    {
                        Directory.CreateDirectory(pathString1);
                    }

            if (!Directory.Exists(pathString2))
                    {
                        Directory.CreateDirectory(pathString2);
                    }

            if (file != null && file.ContentLength > 0)
                
                    {
                        string ext = file.ContentType.ToLower();

                        if (ext != "imgage/jpg" &&
                            ext != "imgage/jpeg" &&
                            ext != "imgage/pjpeg" &&
                            ext != "imgage/gif" &&
                            ext != "imgage/x-png" &&
                            ext != "imgage/png")
                        {
                            using (Db db = new Db())
                            {
                                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                                ModelState.AddModelError("", "Wrong image extention. Upload failed.");
                                return View(model);

                            }
                        }
                        string imageName = file.FileName;

                        using (Db db = new Db())
                        {
                            ProductDTO dto = db.Products.Find(id);
                            dto.ImageName = imageName;
                            db.SaveChanges();

                        }

                        var path = string.Format("{0}\\{1}", pathString2, imageName);
                        var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                        WebImage img = new WebImage(file.InputStream);
                        img.Resize(200, 200);
                        img.Save(path2);
                   }
            
            #endregion
            return RedirectToAction("AddProduct");
            }

     }
}