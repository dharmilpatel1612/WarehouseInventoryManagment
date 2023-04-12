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
                sb.Append("select count(WarehouseSKU) from tblCatalogueProduct where WarehouseSKU='" + catalogue.WarehouseSKU + "'");
                int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                if (Count > 0)
                {
                    // Error message in red color
                    ViewBag.color = "red";
                    ViewBag.ErrorMessage = " Duplicate Warehouse SKU";
                }
                else
                {
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
                        sb.Append("INSERT INTO tblCatalogueProduct ");
                        sb.Append("(ProductName,WarehouseSKU,Description,HSCode,Width,Length,Height,MaterialComposition,VarsanyBag,Colour,");
                        sb.Append("ProductWeight,BoxSize,BoxQuantity,Manufacturer,MailingBag,Size,ProductCost,ShippingCost,Commision,Duty,ProductImage) ");
                        sb.Append("VALUES('" + catalogue.ProductName + "','" + catalogue.WarehouseSKU + "','" + catalogue.Description + "','" + catalogue.HSCode + "',");
                        sb.Append("'" + catalogue.Width + "','" + catalogue.Length + "','" + catalogue.Height + "','" + catalogue.MaterialComposition + "',");
                        sb.Append("'" + catalogue.VarsanyBag + "','" + catalogue.Colour + "','" + catalogue.ProductWeight + "','" + catalogue.BoxSize + "',");
                        sb.Append("'" + catalogue.BoxQuantity + "','" + catalogue.Manufacturer + "','" + catalogue.MaillingBag + "','" + catalogue.Size + "',");
                        sb.Append("'" + catalogue.ProductCost + "','" + catalogue.Shippingncost + "','" + catalogue.Commision + "','" + catalogue.Duty + "','" + imgfileName + "')");
                        obj_dal.EXECUTE_DML(sb.ToString());
                        //string conString = string.Empty;
                        ViewBag.Message = " Data inserted Successfully";

                    }
                    else
                    {

                        sb.Clear();
                        sb.Append("INSERT INTO tblCatalogueProduct ");
                        sb.Append("(ProductName,WarehouseSKU,Description,HSCode,Width,Length,Height,MaterialComposition,VarsanyBag,Colour,");
                        sb.Append("ProductWeight,BoxSize,BoxQuantity,Manufacturer,MailingBag,Size,ProductCost,ShippingCost,Commision,Duty) ");
                        sb.Append("VALUES('" + catalogue.ProductName + "','" + catalogue.WarehouseSKU + "','" + catalogue.Description + "','" + catalogue.HSCode + "',");
                        sb.Append("'" + catalogue.Width + "','" + catalogue.Length + "','" + catalogue.Height + "','" + catalogue.MaterialComposition + "',");
                        sb.Append("'" + catalogue.VarsanyBag + "','" + catalogue.Colour + "','" + catalogue.ProductWeight + "','" + catalogue.BoxSize + "',");
                        sb.Append("'" + catalogue.BoxQuantity + "','" + catalogue.Manufacturer + "','" + catalogue.MaillingBag + "','" + catalogue.Size + "',");
                        sb.Append("'" + catalogue.ProductCost + "','" + catalogue.Shippingncost + "','" + catalogue.Commision + "','" + catalogue.Duty + "')");
                        obj_dal.EXECUTE_DML(sb.ToString());
                        ViewBag.Message = " Data inserted Successfully";
                    }
                }
            }
            catch (Exception ex) 
            {
                ViewBag.color = "red";
                ViewBag.ErrorMessage = ex.Message;

            }
            return View();

        }
       
    }
}