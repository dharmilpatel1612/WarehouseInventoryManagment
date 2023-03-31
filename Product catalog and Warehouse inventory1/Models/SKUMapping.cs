using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace Product_catalog_and_Warehouse_inventory1.Models
{
    public class FileExtensionsValidation:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            if
                (value!=null)
            {
                HttpPostedFileWrapper fileWrapper= (HttpPostedFileWrapper)value;
                string extention = Path.GetExtension(fileWrapper.FileName);
                if(extention!= ".xlsx"& extention!=".xls")
                {
                    return new ValidationResult("Only.xls OR.xlsx excel file allowed");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class SKUMapping
    {
        [Required]
        [FileExtensionsValidation]
        ////[RegularExpression("([a-zA-Z0-9\\s_\\\\.\\-:])+(.xls|.xlsx)$", ErrorMessage = "Only .xls OR .xlsx excel file allowed")]
        public HttpPostedFileBase Uploadfile { get; set; }
        [Required]
        public string WareHouseSKU { get; set; }
        [Required]
        public string MappingSKU { get; set; }
        public List<SKUMapping> MappingList { get; set; }
        public List<SKUMapping> RowMappingSKU { get; set; } = new List<SKUMapping>();
        List<SKUMapping> mappinguserList=new List<SKUMapping>();
    }



}