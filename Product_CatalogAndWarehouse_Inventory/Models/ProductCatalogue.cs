using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class ProductCatalogue
    {
        [Required(ErrorMessage = "Required")] 
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string WarehouseSKU { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid cost")]
        public decimal ProductCost { get; set; }
    }
}