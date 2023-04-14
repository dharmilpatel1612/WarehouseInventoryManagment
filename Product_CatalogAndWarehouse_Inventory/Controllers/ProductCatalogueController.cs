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
        public ActionResult AddCatalogueProduct(ProductCatalogueModel product)
        {
            try
            {
                string uniqueImagename = string.Empty;
                obj_dal = new Dal();
                sb = new StringBuilder();
                // Applied Select query for duplicate value of WarehouseSKU
                sb.Append("SELECT COUNT(WarehouseSKU) FROM tbl_ProductCatalogue WHERE WarehouseSKU = '" + product.WarehouseSKU + "'");
                int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                // If Row count value is grater then 0 so Email id must be duplicate
                if (Count > 0)
                {
                    // Error message in red color
                    ViewBag.ErrorMessage = "Warehouse Already exist";
                }
                else
                {
                    if (product.ImageFile != null)
                    {
                        // For File extension
                        string extention = Path.GetExtension(product.ImageFile.FileName.ToLower());
                        /* if extensions are not .jpg and .png then return view */
                        if (extention != ".jpg" & extention != ".png")
                        {
                            return View();
                        }
                        // Set directory path
                        string path = Server.MapPath("~/UploadedImage/");
                        //if directory is alredy exists then remove it with all file and create new directory
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //Generate new unique image name
                        uniqueImagename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(product.ImageFile.FileName);                       
                        //For file path
                        string filePath = Path.Combine(path, uniqueImagename);
                        //Save as file in given file path
                        product.ImageFile.SaveAs(filePath);
                    }                  
                    sb.Clear();
                    // Used below query to insert all field data into database
                    sb.Append("INSERT INTO tbl_ProductCatalogue (ProductName,WarehouseSKU,Description,HSCode,Width,Length,");
                    sb.Append("Height,MaterialComposition,VarsanyBag,Colour,ProductWeight,BoxSize,BoxQuantity,Manufature,");
                    sb.Append("MailingBag,Size,ProductCost,Commision,ShippingCost,Duty,TotalCost,ProductImage) ");
                    sb.Append("VALUES ('" + product.ProductName + "','" + product.WarehouseSKU + "','" + product.Description + "','" + product.HSCode + "',");
                    sb.Append("'" + product.Width + "','" + product.Length + "','" + product.Height + "','" + product.MaterialComposition + "',");
                    sb.Append("'" + product.VarsanyBag + "','" + product.Colour + "','" + product.ProductWeight + "','" + product.BoxSize + "',");
                    sb.Append("'" + product.BoxQuantity + "','" + product.Manufaturer + "','" + product.MailingBag + "','" + product.Size + "',");
                    sb.Append("'" + product.ProductCost + "','" + product.Commision + "','" + product.ShippingCost + "','" + product.Duty + "',");
                    sb.Append("'" + product.TotalCost + "','" + uniqueImagename + "')");
                    obj_dal.EXECUTE_DML(sb.ToString());
                    // Success message in red color
                    ViewBag.Message = "Data Saved Successfully";
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

