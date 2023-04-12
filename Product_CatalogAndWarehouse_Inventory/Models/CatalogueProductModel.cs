using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class FileExtensionValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                HttpPostedFileWrapper file = (HttpPostedFileWrapper)value;
                string extension = Path.GetExtension(file.FileName);
                if (extension != ".jpg" & extension != ".png")
                {
                    return new ValidationResult("only .jpg and .png file allow");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class CatalogueProductModel
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
        public string Manufacturer { get; set; }
        public string MaillingBag { get; set; }
        public string Size { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal ProductCost { get; set; }
        public decimal Shippingncost { get; set; }
        public decimal Commision { get; set; }
        public decimal Duty { get; set; }
        public decimal ChineseYuan { get; set; }
        public decimal USDollar { get; set; }
        public decimal Euros { get; set; }
        public decimal TotalCost { get; set; }
        [FileExtensionValidate]
        public HttpPostedFileBase ProductImage { get; set; }

    }
}
