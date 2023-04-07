using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class ProductCatalogueController : Controller
    {
        // GET: ProductCatalogue
        public ActionResult AddCatalogueProduct()
        {
            return View();
        }      
    }
}