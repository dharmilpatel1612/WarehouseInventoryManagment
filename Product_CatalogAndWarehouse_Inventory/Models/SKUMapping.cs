using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages.Html;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class FileExtensionsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                HttpPostedFileWrapper file = (HttpPostedFileWrapper)value;
                string extention = Path.GetExtension(file.FileName);

                if (extention != ".xlsx" & extention!=".xls")
                {
                    return new ValidationResult("Only .xls or .xlsx file allowed");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class SKUMapping
    {
        [Required]
        [FileExtensionsValidation]
        public HttpPostedFileBase fileupload { get; set; }    
        public int serialNo { get; set; }
        public string WarehouseSKU { get; set; }
        public string MappingSKU { get; set; }
        public List<SKUMapping> mappingSKUList { get; set; } = new List<SKUMapping>();
        public List<SKUMapping> RowExpansionList { get; set; } = new List<SKUMapping>();
    }
}