using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class MappingSkuModel
    {
        public string MappingSku { get; set; }
        public List<MappingSkuModel> MappingSkuList { get; set; } = new List<MappingSkuModel>();
    }
}