using Microsoft.AspNetCore.Http;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static OfficeOpenXml.ExcelErrorValue;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class ProductCatalogueController : Controller
    {
        Dal obj_dal;
        StringBuilder sb;

        // GET: ProductCatalogue
        public ActionResult AddCatalogueProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCatalogueProduct(ProductCatalogueModel productCatalogue)
        {
            obj_dal = new Dal();
            sb = new StringBuilder();
            try
            {              
                    if (productCatalogue.ImageFile != null)
                    {
                    // For File extension
                    string extention = Path.GetExtension(productCatalogue.ImageFile.FileName.ToLower());
                    /* if extensions are not .jpg and .png then return view */
                    if (extention != ".jpg" & extention != ".png")
                    {
                        return View();
                    }
                    // Set directory path
                    string path = Server.MapPath("~/UploadedImage/");
                    // if directory is alredy exists then remove it with all file and create new directory
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }                  
                    // Generate new unique image name
                    var uniqueImagename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(productCatalogue.ImageFile.FileName);
                    //var uniqueImagename = DateTime.Now.ToString("yyyyMMdd") + "-" + productCatalogue.ImageFile.FileName.Trim();
                    // For file path
                    string filePath = Path.Combine(path, uniqueImagename);
                    // Save as file in given file path
                    productCatalogue.ImageFile.SaveAs(filePath);
                    // Used insert query to store Imagename in database from view
                    sb.Append("INSERT into tbl_ProductCatalogue(ProductImage) VALUES ('" + uniqueImagename + "')");
                    obj_dal.EXECUTE_DML(sb.ToString());
                    //ViewBag.color = "Green";
                    ViewBag.Message = "Save Image Successfully";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View();
        }
    }
}

