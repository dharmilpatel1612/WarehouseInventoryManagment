using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
        //[RegularExpression("([a-zA-Z0-9\\s_\\\\.\\-:])+(.xls|.xlsx)$", ErrorMessage = "Only .xls or .xlsx file allowed")]
        public HttpPostedFileBase fileupload { get; set; }                          
        public string Name { get; set; }
        public string Email { get; set; }
        public List<SKUMapping> List { get; set; }       
    }
}