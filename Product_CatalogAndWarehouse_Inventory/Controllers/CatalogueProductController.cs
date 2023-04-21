using Antlr.Runtime;
using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
        DataTable dt;
        // GET: CatalogueProduct
        public ActionResult CatalogueProduct(string SKU)
        {
            CatalogueProductModel Product = new CatalogueProductModel();
            if (SKU != null)
            {
                obj_dal = new Dal();
                sb = new StringBuilder();
                sb.Append("SELECT ProductName,WarehouseSKU,Description,HSCode,Width,Length,Height,");
                sb.Append("MaterialComposition,VarsanyBag,Colour,ProductWeight,BoxSize,BoxQuantity,");
                sb.Append("Manufacturer,MailingBag,Size,ProductCost,ShippingCost,");
                sb.Append("Commision,Duty,TotalCost,ProductImage ");
                sb.Append("FROM tblCatalogueProduct WHERE WarehouseSKU='" + SKU + "'");
                dt = obj_dal.GET_DATATABLE(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    // code to be executed repeatedly until row count less than 0
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        // Get the value of ProductName
                        Product.ProductName = dt.Rows[i]["ProductName"].ToString();
                        // Get the value of WarehouseSKU
                        Product.WarehouseSKU = dt.Rows[i]["WarehouseSKU"].ToString();
                        // Get the value of Description
                        Product.Description = dt.Rows[i]["Description"].ToString();
                        // Get the value of HSCode
                        Product.HSCode = dt.Rows[i]["HSCode"].ToString();
                        // Get the value of Width
                        Product.Width = Convert.ToDecimal(dt.Rows[i]["Width"].ToString());
                        // Get the value of Length
                        Product.Length = Convert.ToDecimal(dt.Rows[i]["Length"].ToString());
                        // Get the value of Height
                        Product.Height = Convert.ToDecimal(dt.Rows[i]["Height"].ToString());
                        // Get the value of MaterialComposition
                        Product.MaterialComposition = dt.Rows[i]["MaterialComposition"].ToString();
                        // Get the value of VarsanyBag
                        Product.VarsanyBag = dt.Rows[i]["VarsanyBag"].ToString();
                        // Get the value of Colour
                        Product.Colour = dt.Rows[i]["Colour"].ToString();
                        // Get the value of ProductWeight
                        Product.ProductWeight = Convert.ToDecimal(dt.Rows[i]["ProductWeight"].ToString());
                        // Get the value of BoxSize
                        Product.BoxSize = Convert.ToDecimal(dt.Rows[i]["BoxSize"].ToString());
                        // Get the value of BoxQuantity
                        Product.BoxQuantity = Convert.ToInt32(dt.Rows[i]["BoxQuantity"].ToString());
                        // Get the value of Manufacturer
                        Product.Manufacturer = dt.Rows[i]["Manufacturer"].ToString();
                        // Get the value of MaillingBag 
                        Product.MailingBag = dt.Rows[i]["MailingBag"].ToString();
                        // Get the value of Size
                        Product.Size = dt.Rows[i]["Size"].ToString();
                        // Get the value of ProductCost
                        Product.ProductCost = Convert.ToDecimal(dt.Rows[i]["ProductCost"].ToString());
                        // Get the value of Shippingcost
                        Product.Shippingcost = Convert.ToDecimal(dt.Rows[i]["Shippingcost"].ToString());
                        // Get the value of Commision
                        Product.Commision = Convert.ToDecimal(dt.Rows[i]["Commision"].ToString());
                        // Get the value of Duty
                        Product.Duty = Convert.ToDecimal(dt.Rows[i]["Duty"].ToString());
                        // Get the value of TotalCost
                        Product.TotalCost = Convert.ToDecimal(dt.Rows[i]["TotalCost"].ToString());
                        // Get the value of ProductImage
                        Product.image = dt.Rows[i]["ProductImage"].ToString();
                        ViewBag.imageurl = Product.image;
                        TempData["Message"] = "Update Product";
                        ViewBag.Message1 = "Edit Catalogue Product";
                    }
                }
                return View(Product);
            }
            ViewBag.Message1 = "Add Catalogue Product";
            TempData["Message"] = "SaveProduct";
            return View();
        }

        [HttpPost]
        public ActionResult CatalogueProduct(CatalogueProductModel catalogue, string SKU)
        {
            try
            {
                string uploadedImageName = string.Empty;
                // If statement condition used for image
                if (catalogue.ProductImage != null)
                {
                    // Set file path with directory
                    var path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    uploadedImageName = Guid.NewGuid().ToString() + Path.GetExtension(catalogue.ProductImage.FileName);

                    string filepath = Path.Combine(path, uploadedImageName);
                    catalogue.ProductImage.SaveAs(filepath);
                }
                //Update code
                if (SKU != null)
                {
                    catalogue.Width = catalogue.Width ?? 0;
                    catalogue.Length = catalogue.Length ?? 0;
                    catalogue.Height = catalogue.Height ?? 0;
                    catalogue.ProductWeight = catalogue.ProductWeight ?? 0;
                    catalogue.BoxSize = catalogue.BoxSize ?? 0;
                    catalogue.BoxQuantity = catalogue.BoxQuantity ?? 0;
                    catalogue.Commision = catalogue.Commision ?? 0;
                    catalogue.Shippingcost = catalogue.Shippingcost ?? 0;
                    catalogue.Duty = catalogue.Duty ?? 0;
                    obj_dal = new Dal();
                    sb = new StringBuilder();
                    // Used to Applied SELECT Query to count WarehouseSKU data from database using model
                    sb.Append("select count(WarehouseSKU) from tblCatalogueProduct where WarehouseSKU='" + catalogue.WarehouseSKU + "'");
                    /* Variable declaration and convert into int
                       Get_SingleValue function call from Dal class to return single value*/
                    int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                    // If Row count value is grater then 0 so warehousesku must be duplicate
                    if (Count > 1)
                    {
                        // Error message
                        ViewBag.ErrorMessage = " Duplicate Warehouse SKU";
                    }
                    // update code
                    else
                    {
                        sb.Clear();
                        sb.Append("SELECT ProductImage from tblCatalogueProduct where WarehouseSKU='" + SKU + "'");
                        string existingImageName = obj_dal.Get_SingleValue(sb.ToString());
                        if (existingImageName != uploadedImageName && uploadedImageName != "")
                        {
                            // Set file path with directory
                            string path = Server.MapPath("/Uploads/" + existingImageName);
                            FileInfo file = new FileInfo(path);
                            if (file.Exists)//check file exsit or not  
                            {
                                file.Delete();
                            }
                        }
                        sb.Clear();
                        sb.Append("UPDATE tblCatalogueProduct SET ProductName='" + catalogue.ProductName + "',WarehouseSKU='" + catalogue.WarehouseSKU + "',");
                        sb.Append("Description='" + catalogue.Description + "',HSCode='" + catalogue.HSCode + "',Width='" + catalogue.Width + "',");
                        sb.Append("Length='" + catalogue.Length + "',Height='" + catalogue.Height + "',MaterialComposition='" + catalogue.MaterialComposition + "',");
                        sb.Append("VarsanyBag='" + catalogue.VarsanyBag + "',Colour='" + catalogue.Colour + "',ProductWeight='" + catalogue.ProductWeight + "',");
                        sb.Append("BoxSize='" + catalogue.BoxSize + "',BoxQuantity='" + catalogue.BoxQuantity + "',Manufacturer='" + catalogue.Manufacturer + "',");
                        sb.Append("MailingBag='" + catalogue.MailingBag + "',Size='" + catalogue.Size + "',ProductCost='" + catalogue.ProductCost + "',");
                        sb.Append("ShippingCost='" + catalogue.Shippingcost + "',Commision='" + catalogue.Commision + "',Duty='" + catalogue.Duty + "',");
                        //not image update only img name display use in select query
                        if (!string.IsNullOrEmpty(uploadedImageName))
                        {
                            //image update
                            sb.Append("ProductImage='" + uploadedImageName + "',");
                        }
                        else
                        {
                            existingImageName = obj_dal.Get_SingleValue("SELECT ProductImage from tblCatalogueProduct where WarehouseSKU='" + SKU + "'");
                            uploadedImageName = existingImageName;
                        }
                        sb.Append("TotalCost='" + catalogue.TotalCost + "' ");
                        sb.Append("FROM tblCatalogueProduct WHERE WarehouseSKU='" + SKU + "'");
                        obj_dal.EXECUTE_DML(sb.ToString());
                        //image display
                        ViewBag.imageurl = uploadedImageName;
                        ViewBag.Message = "Data updated successfully!";
                        TempData["Message"] = "Update Product";
                        ViewBag.Message1 = "Update Catalogue Product";
                    }
                }
                //insert code
                else
                {
                    catalogue.Width = catalogue.Width ?? 0;
                    catalogue.Length = catalogue.Length ?? 0;
                    catalogue.Height = catalogue.Height ?? 0;
                    catalogue.ProductWeight = catalogue.ProductWeight ?? 0;
                    catalogue.BoxSize = catalogue.BoxSize ?? 0;
                    catalogue.BoxQuantity = catalogue.BoxQuantity ?? 0;
                    catalogue.Commision = catalogue.Commision ?? 0;
                    catalogue.Shippingcost = catalogue.Shippingcost ?? 0;
                    catalogue.Duty = catalogue.Duty ?? 0;
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
                    //insert code data and image insert
                    else
                    {
                        // If statement condition used for image and insert data  
                        if (catalogue.ProductImage != null)
                        {
                            // Set file path with directory
                            var path = Server.MapPath("~/Uploads/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            uploadedImageName = Guid.NewGuid().ToString() + Path.GetExtension(catalogue.ProductImage.FileName);
                            string filepath = Path.Combine(path, uploadedImageName);
                            catalogue.ProductImage.SaveAs(filepath);
                            //DataTable dt;
                            sb.Clear();
                            // Used to applied INSERT Query to get data from model to database
                            sb.Append("INSERT INTO tblCatalogueProduct ");
                            sb.Append("(ProductName,WarehouseSKU,Description,HSCode,Width,Length,Height,MaterialComposition,VarsanyBag,Colour,");
                            sb.Append("ProductWeight,BoxSize,BoxQuantity,Manufacturer,MailingBag,Size,ProductCost,ShippingCost,Commision,Duty,ProductImage,TotalCost) ");
                            sb.Append("VALUES('" + catalogue.ProductName + "','" + catalogue.WarehouseSKU + "','" + catalogue.Description + "','" + catalogue.HSCode + "',");
                            sb.Append("'" + catalogue.Width + "','" + catalogue.Length + "','" + catalogue.Height + "','" + catalogue.MaterialComposition + "',");
                            sb.Append("'" + catalogue.VarsanyBag + "','" + catalogue.Colour + "','" + catalogue.ProductWeight + "','" + catalogue.BoxSize + "',");
                            sb.Append("'" + catalogue.BoxQuantity + "','" + catalogue.Manufacturer + "','" + catalogue.MailingBag + "','" + catalogue.Size + "',");
                            sb.Append("'" + catalogue.ProductCost + "','" + catalogue.Shippingcost + "','" + catalogue.Commision + "','" + catalogue.Duty + "','" + uploadedImageName + "','" + catalogue.TotalCost + "')");
                            // EXECUTE_DM function call from Dal class to execute sql query
                            obj_dal.EXECUTE_DML(sb.ToString());
                            ViewBag.imageurl = uploadedImageName;
                            //success message
                            ViewBag.Message = " Data inserted Successfully";
                            TempData["Message"] = "SaveProduct";
                            ViewBag.Message1 = "Add Catalogue Product";
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
                            sb.Append("'" + catalogue.BoxQuantity + "','" + catalogue.Manufacturer + "','" + catalogue.MailingBag + "','" + catalogue.Size + "',");
                            sb.Append("'" + catalogue.ProductCost + "','" + catalogue.Shippingcost + "','" + catalogue.Commision + "','" + catalogue.Duty + "','" + catalogue.TotalCost + "')");
                            // EXECUTE_DM function call from Dal class to execute sql query
                            obj_dal.EXECUTE_DML(sb.ToString());
                            //success message
                            ViewBag.Message = " Data inserted Successfully";
                            TempData["Message"] = "SaveProduct";
                            ViewBag.Message1 = "Add Catalogue Product";
                        }
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
        //Remove image section
        public JsonResult RemoveImage(string warehousesku)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                obj_dal = new Dal();
                //string existingImageName = string.Empty;
                // Used to Applied SELECT Query to ProductImage data from database 
                sb.Append("SELECT ProductImage from tblCatalogueProduct where WarehouseSKU='" + warehousesku + "'");
                string existingImageName = obj_dal.Get_SingleValue(sb.ToString());
                if (!string.IsNullOrEmpty(existingImageName))
                {
                    // Set file path with directory
                    string path = Server.MapPath("/Uploads/" + existingImageName);
                    FileInfo file = new FileInfo(path);
                    if (file.Exists)//check file exsit or not  
                    {
                        file.Delete();
                    }
                    sb.Clear();
                    // Used to Applied UPDATE Query to ProductImage 
                    sb.Append("UPDATE tblCatalogueProduct SET ProductImage=''");
                    sb.Append("FROM tblCatalogueProduct WHERE WarehouseSKU='" + warehousesku + "'");
                    obj_dal.EXECUTE_DML(sb.ToString());
                }
                return Json(new { iserror = false, result = 1, message = "File successfully deleted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { iserror = true, result = 1, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}