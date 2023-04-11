using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class ImageExtensionsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                HttpPostedFileWrapper file = (HttpPostedFileWrapper)value;
                string extention = Path.GetExtension(file.FileName);

                if (extention != ".jpg" & extention != ".png")
                {
                    return new ValidationResult("Only .jpg or .png file allowed");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class ProductCatalogueModel
    {
        [Required(ErrorMessage = "Required")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string WarehouseSKU { get; set; }
        public string Description { get; set; }
        public string HSCode { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public string MaterialComposition { get; set; }
        public string VarsanyBag { get; set; }
        public string Colour { get; set; }
        public decimal ProductWeight { get; set; }
        public decimal BoxSize { get; set; }
        public int BoxQuantity { get; set; }
        public string Manufaturer { get; set; }  
        public string MailingBag { get; set; }
        public string Size { get;set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid cost")]
        public decimal ProductCost { get; set; }
        public decimal Commision { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Duty { get; set; }
        public decimal TotalCost { get; set;}

        [ImageExtensionsValidation]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}