﻿using SklepDot.Areas.Administrator.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SklepDot.Areas.Administrator.Models.ViewModels.Pages
{
    public class ProductVM
    {
        public ProductVM()
        {
        }

        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;

        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}