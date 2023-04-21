using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_catalog_and_Warehouse_inventory1.Models
{
    public class CatelogueModel
    { 
       

        [Required(ErrorMessage = "Required")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string WarehouseSKU { get; set; }
        public string Description { get; set; }
        public string HSCode { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? Height { get; set; }
        public string MaterialComposition { get; set; }
        public string VarsanyBag { get; set; }
        public string Colour { get; set; }
        public decimal? ProductWeight { get; set; }
        public decimal? BoxSize { get; set; }
        public int? BoxQuantity { get; set; }
        public string Manufacturer { get; set; }
        public string MaillingBag { get; set; }
        public string Size { get; set; }
        [DisplayName("ProductCost")]
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[0-9]*\\.?[0-9]+$", ErrorMessage = "Please enter valid ProductCost.")]
        public decimal? ProductCost { get; set; }
        
        public decimal? Shippingncost { get; set; }
        
        public decimal? Commision { get; set; }
        
        public decimal? Duty { get; set; }
        public decimal? ChineseYuan { get; set; }
        public decimal? USDollar { get; set; }
        public decimal? Euros { get; set; }
        public decimal? TotalCost { get; set; }
      
        public HttpPostedFileBase ProductImage { get; set; }
        public string FileImage { get; set; }
        public List<CatelogueModel> productList { get; set; }
    }
}