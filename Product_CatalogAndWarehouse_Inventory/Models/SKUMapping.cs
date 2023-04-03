using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class FileExtensionValidation : ValidationAttribute
    { 
        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            if(value != null)
            {
                HttpPostedFileWrapper file= (HttpPostedFileWrapper)value;
                string extension =Path.GetExtension(file.FileName);
                if(extension != ".xlsx" & extension != ".xls")
                {
                    return new ValidationResult("only .xls and .xlsx file allow");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class SKUMapping
    {
        [Required]
        [FileExtensionValidation]
        //[RegularExpression("([a-zA-Z0-9\\s_\\\\.\\-:])+(.xls|.xlsx)$", ErrorMessage ="only .xls and .xlsx file allow")]
        public HttpPostedFileBase Fileupload { get; set; }

        [Required]
        public string WarehouseSKU { get; set; }

        [Required]
        [MaxLength(100)]
        public string MappingSKU { get; set; }

        [Required]
        public string idWarehouseSKU {get; set; }

        public List<SKUMapping> MappingList { get; set; } = new List<SKUMapping>();
        public List<SKUMapping> MappingRowList { get; set; } = new List<SKUMapping>();


    }

}