﻿using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Windows.Forms;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class ProductCatalogueController : Controller
    {
        Dal obj_dal;
        StringBuilder sb;
        DataTable dt_product = new DataTable();

        // GET: Edit Product Catalogue
        public ActionResult AddCatalogueProduct(string sku)              
        {          
            ProductCatalogueModel productmodel = new ProductCatalogueModel();
            /* If statement is used to check sku is null or not 
               if it is not null than fill AddCatalogueProduct page fieldwith corresponding WarehouseSKU*/
            if (sku != null)
            {      
                obj_dal = new Dal();
                sb = new StringBuilder();
                // Applied SELECT Query to get data of tbl_ProductCatalogue from database with the use of warehousesku
                sb.Append("SELECT ProductName, WarehouseSKU, Description, HSCode, Width, Length, Height, MaterialComposition,");
                sb.Append("VarsanyBag, Colour, ProductWeight, BoxSize, BoxQuantity, Manufature, MailingBag, Size,");
                sb.Append("ProductCost, Commision, ShippingCost, Duty, TotalCost, ProductImage ");
                sb.Append("FROM tbl_ProductCatalogue  WHERE WarehouseSKU = '" + sku + "'");
                dt_product = obj_dal.GET_DATATABLE(sb.ToString());
                // Used to check row count grater than 0
                if (dt_product.Rows.Count > 0)
                {
                    // code to be executed repeatedly until row count less than 0
                    for (int i = 0; i < dt_product.Rows.Count; i++)
                    {
                        // Get the value of all Add Catelogue Product page fields
                        productmodel.ProductName = dt_product.Rows[i]["ProductName"].ToString();                       
                        productmodel.WarehouseSKU = dt_product.Rows[i]["WarehouseSKU"].ToString();                      
                        productmodel.Description = dt_product.Rows[i]["Description"].ToString();                      
                        productmodel.HSCode = dt_product.Rows[i]["HSCode"].ToString();                      
                        productmodel.Width = Convert.ToDecimal(dt_product.Rows[i]["Width"].ToString());
                        productmodel.Length = Convert.ToDecimal(dt_product.Rows[i]["Length"].ToString());
                        productmodel.Height = Convert.ToDecimal(dt_product.Rows[i]["Height"].ToString());                       
                        productmodel.MaterialComposition = dt_product.Rows[i]["MaterialComposition"].ToString();                      
                        productmodel.VarsanyBag = dt_product.Rows[i]["VarsanyBag"].ToString();
                        productmodel.Colour = dt_product.Rows[i]["Colour"].ToString();
                        productmodel.ProductWeight = Convert.ToDecimal(dt_product.Rows[i]["ProductWeight"].ToString());
                        productmodel.BoxSize = Convert.ToDecimal(dt_product.Rows[i]["BoxSize"].ToString());
                        productmodel.BoxQuantity = Convert.ToInt32(dt_product.Rows[i]["BoxQuantity"].ToString());
                        productmodel.Manufaturer = dt_product.Rows[i]["Manufature"].ToString();
                        productmodel.MailingBag = dt_product.Rows[i]["MailingBag"].ToString();
                        productmodel.Size = dt_product.Rows[i]["Size"].ToString();                       
                        productmodel.ProductCost = Convert.ToDecimal(dt_product.Rows[i]["ProductCost"].ToString());
                        productmodel.Commision = Convert.ToDecimal(dt_product.Rows[i]["Commision"].ToString());
                        productmodel.ShippingCost = Convert.ToDecimal(dt_product.Rows[i]["ShippingCost"].ToString());
                        productmodel.Duty = Convert.ToDecimal(dt_product.Rows[i]["Duty"].ToString());
                        productmodel.TotalCost = Convert.ToDecimal(dt_product.Rows[i]["TotalCost"].ToString());
                        ViewBag.ImageUrl = productmodel.FileImage = dt_product.Rows[i]["ProductImage"].ToString();
                    }
                }
                ViewBag.ProductName = productmodel.ProductName;
                TempData["Message"] = "Update Product";
                return View(productmodel);
            }
            ViewBag.Title = "Add Catalogue Product";
            TempData["Message"] = "Save Product";
            return View();
        }

        // POST : ADD / Edit Product Catalogue 
        [HttpPost]
        public ActionResult AddCatalogueProduct(ProductCatalogueModel product,string sku)
        {
            try
            {
                string uniqueImagename = string.Empty;
                obj_dal = new Dal();
                sb = new StringBuilder();
                product.Width = product.Width ?? 0;
                product.Length = product.Length ?? 0;
                product.Height = product.Height ?? 0;
                product.ProductWeight = product.ProductWeight ?? 0;
                product.BoxSize = product.BoxSize ?? 0;
                product.BoxQuantity = product.BoxQuantity ?? 0;
                product.Commision = product.Commision ?? 0;
                product.ShippingCost = product.ShippingCost ?? 0;
                product.Duty = product.Duty ?? 0;
                
                /* If statement is used to check ImageFile is null or not 
                 * if it is not null than save image in UploadedIMage folder.*/
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
                /* If statement is used to check sku is null or not 
                 * if it is not null than Update Add catelogue product page field
                 * otherwise insert Add catelogue product page field data into database.*/
                if (sku != null)
                {
                    // Applied Select query for duplicate value of WarehouseSKU
                    sb.Append("SELECT COUNT(WarehouseSKU) FROM tbl_ProductCatalogue WHERE WarehouseSKU = '" + product.WarehouseSKU + "' AND WarehouseSKU!='"+sku+"'");
                    int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                    // If Row count value is grater then 0 so Email id must be duplicate
                    if (Count > 0)
                    {
                        // Error message in red color
                        ViewBag.ErrorMessage = "Warehouse Already exist";
                    }
                    else
                    {                       
                        sb.Clear();
                        // Applied Select query for get value of ProductImage
                        sb.Append("SELECT ProductImage FROM tbl_ProductCatalogue where WarehouseSKU = '" + sku + "'");
                        string Image = obj_dal.Get_SingleValue(sb.ToString());
                        /*If statement use for delete old image file when update image.
                         *else statement use for pass image file name to database */
                        if (Image != uniqueImagename && uniqueImagename != "")
                        {
                            // Set file path with directory
                            string path = Server.MapPath("~/UploadedImage/" + Image);
                            FileInfo file = new FileInfo(path);
                            //check file exists or not.if file exists then it will delete file 
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                        else
                        {
                            uniqueImagename = Image;
                        }
                        sb.Clear();
                        // Applied UPDATE Query to update Add catelogue product page field in database using model
                        sb.Append("UPDATE tbl_ProductCatalogue SET ProductName = '"+product.ProductName+ "',WarehouseSKU = '"+product.WarehouseSKU+"',");
                        sb.Append("Description='" + product.Description+ "',HSCode='"+product.HSCode+ "',Width='" + product.Width+ "',");
                        sb.Append("Length='"+product.Length+ "',Height='"+product.Height+"', MaterialComposition='"+product.MaterialComposition+"',");
                        sb.Append("VarsanyBag='"+product.VarsanyBag+ "',Colour='"+product.Colour+ "',ProductWeight='"+product.ProductWeight+"',");
                        sb.Append("BoxSize='"+product.BoxSize+ "',BoxQuantity='"+product.BoxQuantity+ "',Manufature='"+product.Manufaturer+"',");
                        sb.Append("MailingBag='" + product.MailingBag+"',Size='" + product.Size+ "',ProductCost='"+product.ProductCost+ "',");
                        sb.Append("Commision='"+product.Commision+"',ShippingCost='" + product.ShippingCost+ "',Duty='"+product.Duty+ "',");
                        sb.Append("TotalCost='"+product.TotalCost+"',ProductImage='" + uniqueImagename + "' WHERE WarehouseSKU = '" + sku + "'");
                        obj_dal.EXECUTE_DML(sb.ToString());
                        ViewBag.ImageUrl = uniqueImagename;
                        ViewBag.ProductName = product.ProductName;
                        // Success message in red color
                        ViewBag.Message = "Data updated successfully!";                 
                    }
                    TempData["Message"] = "Update Product";
                }
                else
                {
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
                        // Success message in green color
                        ViewBag.Message = "Data Saved Successfully";
                        ViewBag.ImageUrl = uniqueImagename;
                        TempData["Message"] = "Save Product";
                    }
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

