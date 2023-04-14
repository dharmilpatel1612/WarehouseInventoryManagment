using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class CatalogueProductController : Controller
    {
        StringBuilder sb;
        Dal obj_dal;
        // GET: CatalogueProduct
        public ActionResult CatalogueProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CatalogueProduct(CatalogueProductModel catalogue)
        {
            try 
            {
                obj_dal = new Dal();
                sb = new StringBuilder();
                // Used to Applied SELECT Query to count WarehouseSKU data from database using model
                sb.Append("select count(WarehouseSKU) from tblCatalogueProduct where WarehouseSKU='" + catalogue.WarehouseSKU + "'");
                /* Variable declaration and convert into int
                   Get_SingleValue function call from Dal class to return single value*/
                int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                // If Row count value is grater then 0 so Email id must be duplicate
                if (Count > 0)
                {
                    // Error message
                    ViewBag.ErrorMessage = " Duplicate Warehouse SKU";
                }
                else
                {
                    // If statement condition used for image and insert data  
                    if (catalogue.ProductImage != null)
                    {
                        var path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var imgfileName = Guid.NewGuid().ToString() + Path.GetExtension(catalogue.ProductImage.FileName);

                        string filepath = Path.Combine(path, imgfileName);
                        catalogue.ProductImage.SaveAs(filepath);
                        sb.Clear();
                        // Used to applied INSERT Query to get data from model to database
                        sb.Append("INSERT INTO tblCatalogueProduct ");
                        sb.Append("(ProductName,WarehouseSKU,Description,HSCode,Width,Length,Height,MaterialComposition,VarsanyBag,Colour,");
                        sb.Append("ProductWeight,BoxSize,BoxQuantity,Manufacturer,MailingBag,Size,ProductCost,ShippingCost,Commision,Duty,ProductImage,TotalCost) ");
                        sb.Append("VALUES('" + catalogue.ProductName + "','" + catalogue.WarehouseSKU + "','" + catalogue.Description + "','" + catalogue.HSCode + "',");
                        sb.Append("'" + catalogue.Width + "','" + catalogue.Length + "','" + catalogue.Height + "','" + catalogue.MaterialComposition + "',");
                        sb.Append("'" + catalogue.VarsanyBag + "','" + catalogue.Colour + "','" + catalogue.ProductWeight + "','" + catalogue.BoxSize + "',");
                        sb.Append("'" + catalogue.BoxQuantity + "','" + catalogue.Manufacturer + "','" + catalogue.MaillingBag + "','" + catalogue.Size + "',");
                        sb.Append("'" + catalogue.ProductCost + "','" + catalogue.Shippingncost + "','" + catalogue.Commision + "','" + catalogue.Duty + "','" + imgfileName + "','"+catalogue.TotalCost+"')");
                        // EXECUTE_DM function call from Dal class to execute sql query
                        obj_dal.EXECUTE_DML(sb.ToString());
                        //success message
                        ViewBag.Message = " Data inserted Successfully";

                    }
                    // else statement condition used for insert data 
                    else
                    {

                        sb.Clear();
                        // Used to applied INSERT Query to get data from model to database
                        sb.Append("INSERT INTO tblCatalogueProduct ");
                        sb.Append("(ProductName,WarehouseSKU,Description,HSCode,Width,Length,Height,MaterialComposition,VarsanyBag,Colour,");
                        sb.Append("ProductWeight,BoxSize,BoxQuantity,Manufacturer,MailingBag,Size,ProductCost,ShippingCost,Commision,Duty,TotalCost) ");
                        sb.Append("VALUES('" + catalogue.ProductName + "','" + catalogue.WarehouseSKU + "','" + catalogue.Description + "','" + catalogue.HSCode + "',");
                        sb.Append("'" + catalogue.Width + "','" + catalogue.Length + "','" + catalogue.Height + "','" + catalogue.MaterialComposition + "',");
                        sb.Append("'" + catalogue.VarsanyBag + "','" + catalogue.Colour + "','" + catalogue.ProductWeight + "','" + catalogue.BoxSize + "',");
                        sb.Append("'" + catalogue.BoxQuantity + "','" + catalogue.Manufacturer + "','" + catalogue.MaillingBag + "','" + catalogue.Size + "',");
                        sb.Append("'" + catalogue.ProductCost + "','" + catalogue.Shippingncost + "','" + catalogue.Commision + "','" + catalogue.Duty + "','"+catalogue.TotalCost+"')");
                        // EXECUTE_DM function call from Dal class to execute sql query
                        obj_dal.EXECUTE_DML(sb.ToString());
                        //success message
                        ViewBag.Message = " Data inserted Successfully";
                    }
                }
            }
            catch (Exception ex) 
            {
                //Error message
                ViewBag.ErrorMessage = ex.Message;

            }
            return View();

        }
       
    }
}