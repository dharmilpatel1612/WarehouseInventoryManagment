using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class CatalogueProductModel
    {
        [Required(ErrorMessage = "Required")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string WarehouseSKU { get; set; }
        public string Description { get; set; }
        public string HSCode { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public string MaterialComposition { get; set; }
        public string VarsanyBag { get; set; }
        public string Colour { get; set; }
        public int ProductWeight { get; set; }
        public int BoxSize { get; set; }
        public int BoxQuantity { get; set; }
        public int Manufacturer { get; set; }
        public int MaillingBag { get; set; }
        public int Size { get; set; }
        [Required(ErrorMessage = "Required")]
        public int ProductCost { get; set; }
        public int Shippingncost { get; set; }
        public int Commision { get; set; }
        public int Duty { get; set; }
        public int ChineseYuan { get; set; }
        public int USDollar { get; set; }
        public int Euros { get; set; }
        public int TotalCost { get; set; }

    }
}
