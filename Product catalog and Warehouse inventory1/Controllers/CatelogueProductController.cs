using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Product_catalog_and_Warehouse_inventory1.Models;
using System.Globalization;
using System.Xml.Schema;
using SolrNet.Utils;
using System.Configuration;
using System.Web.Services.Description;
using Microsoft.SqlServer.Server;
using System.Web.WebPages;

namespace Product_catalog_and_Warehouse_inventory1.Controllers
{
    public class CatelogueProductController : Controller
    {
        //object 
        SqlCommand cmd;
        StringBuilder sb;
        Dal objDal;
        DataTable dt;


        // GET: CatelogueProduct
        public ActionResult catelogueproduct(string WarehouseSKU)

        {
            objDal = new Dal();

            if (WarehouseSKU != null)
            {
                CatelogueModel usermodel = new CatelogueModel();
                sb = new StringBuilder();
                sb.Append("select Product_Name,WarehouseSKU,Description,HSCode,Width,Length,Height,MaterialComposition,");
                sb.Append("VarsanyBag,Colour,ProductWeight,BoxSize,BoxQuenitity,Manufactuer,MailingBag,Size,ProductCost,");
                sb.Append("ShippingCost,Comission,Duty,TotalCost,ProductImage from tblCatelogue ");
                sb.Append("where WarehouseSKU='" + WarehouseSKU + "'");
                objDal.EXECUTE_DML(sb.ToString());
                //GetDataTable function call from dal class to fill the table data 
                dt = objDal.GetDataTable(sb.ToString());
                List<CatelogueModel> productList = new List<CatelogueModel>();

                //Used to check row count grater then 0
                if (dt.Rows.Count > 0)
                {
                    //code to be executed repeatedly util row i less then 0
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //var usermodel = new CatelogueModel();
                        usermodel.ProductName = dt.Rows[i]["Product_Name"].ToString();
                        usermodel.WarehouseSKU = dt.Rows[i]["WarehouseSKU"].ToString();
                        usermodel.Description = dt.Rows[i]["Description"].ToString();
                        usermodel.HSCode = dt.Rows[i]["HSCode"].ToString();
                        usermodel.Width = Convert.ToDecimal(dt.Rows[i]["Width"].ToString());
                        usermodel.Length = Convert.ToDecimal(dt.Rows[i]["Length"].ToString());
                        usermodel.Height = Convert.ToDecimal(dt.Rows[i]["Height"].ToString());
                        usermodel.MaterialComposition = dt.Rows[i]["MaterialComposition"].ToString();
                        usermodel.VarsanyBag = dt.Rows[i]["VarsanyBag"].ToString();
                        usermodel.Colour = dt.Rows[i]["Colour"].ToString();
                        usermodel.ProductWeight = Convert.ToDecimal(dt.Rows[i]["ProductWeight"].ToString());
                        usermodel.BoxSize = Convert.ToDecimal(dt.Rows[i]["BoxSize"].ToString());
                        usermodel.BoxQuantity = Convert.ToInt32(dt.Rows[i]["BoxQuenitity"].ToString());
                        usermodel.Manufacturer = dt.Rows[i]["Manufactuer"].ToString();
                        usermodel.MaillingBag = dt.Rows[i]["MailingBag"].ToString();
                        usermodel.Size = dt.Rows[i]["Size"].ToString();
                        usermodel.ProductCost = Convert.ToDecimal(dt.Rows[i]["ProductCost"].ToString());
                        usermodel.Shippingncost = Convert.ToDecimal(dt.Rows[i]["ShippingCost"].ToString());
                        usermodel.Commision = Convert.ToDecimal(dt.Rows[i]["Comission"].ToString());
                        usermodel.Duty = Convert.ToDecimal(dt.Rows[i]["Duty"].ToString());
                        usermodel.TotalCost = Convert.ToDecimal(dt.Rows[i]["TotalCost"].ToString());
                        ViewBag.imageurl = usermodel.FileImage = dt.Rows[i]["ProductImage"].ToString();
                        //productList.Add(usermodel);

                    }

                }
                ViewBag.Title = "Update Catelogue Product";
                TempData["Message"] = "Update Product";
                return View(usermodel);

            }
            ViewBag.Title = "Add Catelogue Product";
            TempData["Message"] = "Save Product";
            return View();

        }
        [HttpPost]
        public ActionResult catelogueproduct(CatelogueModel usermodel,string WarehouseSKU)
        {

            objDal = new Dal();
            sb = new StringBuilder();
            string imgfileName = string.Empty;
            string filePath = string.Empty;
            if (usermodel.ProductImage != null)
            {

                string path = Server.MapPath("~/upload image/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(usermodel.ProductImage.FileName);
                string extension = Path.GetExtension(usermodel.ProductImage.FileName);
                //usermodel.ProductImage.SaveAs(filePath);
                string[] allowextension = new string[] { ".JPG", ".PNG", ".jpg", ".png", ".jpeg", ".gif", ".tiff", ".eps", ".ai", ".psd" };

                string conString = string.Empty;
                imgfileName = Guid.NewGuid().ToString() + Path.GetExtension(usermodel.ProductImage.FileName);

                string Filepath = Path.Combine(path, imgfileName);
                usermodel.ProductImage.SaveAs(Filepath);
            }

                sb.Append("select count(WarehouseSKU) from tblCatelogue where WarehouseSKU='" + WarehouseSKU + "'");
            int Count = Convert.ToInt32(objDal.Get_SingleValue(sb.ToString()));
            if (Count > 1)
            {
                // Error message in red color
                ViewBag.color = "red";
                ViewBag.ErrorMessage = " Duplicate Warehouse SKU";
            }
            //CatelogueModel usermodel = new CatelogueModel();
            else
            {
                usermodel.Width = usermodel.Width ?? 0;
                usermodel.Length = usermodel.Length ?? 0;
                usermodel.Height = usermodel.Height ?? 0;
                usermodel.ProductWeight = usermodel.ProductWeight ?? 0;
                usermodel.BoxSize = usermodel.BoxSize ?? 0;
                usermodel.BoxQuantity = usermodel.BoxQuantity ?? 0;
                usermodel.ProductCost = usermodel.ProductCost ?? 0;
                usermodel.Shippingncost = usermodel.Shippingncost ?? 0;
                usermodel.Commision = usermodel.Commision ?? 0;
                usermodel.Duty = usermodel.Duty ?? 0;
                usermodel.TotalCost = usermodel.TotalCost ?? 0;
                

                if (Count>0)
                {
                    sb.Clear();
                    sb.Append("select ProductImage from tblCatelogue where WarehouseSKU='" + WarehouseSKU + "'");
                    string image = objDal.Get_SingleValue(sb.ToString());
                    //imgfileName = image;
                    if (imgfileName != image && imgfileName != "")
                    {
                        var filePath1 = Server.MapPath("~/upload image/" + image);
                        FileInfo files = new FileInfo(filePath1); 
                        if (files.Exists)
                        {
                            files.Delete();
                        }
                    }
                    else
                    {
                        imgfileName = image;
                    }
                    sb.Clear();
                    sb = new StringBuilder();
                    sb.Append("update tblCatelogue set Product_Name='" + usermodel.ProductName + "',WarehouseSKU='" + usermodel.WarehouseSKU + "',");
                    sb.Append("Description='" + usermodel.Description + "',HSCode='" + usermodel.HSCode + "',Width='" + usermodel.Width + "',");
                    sb.Append("Length='" + usermodel.Length + "',Height='" + usermodel.Height + "',MaterialComposition='" + usermodel.MaterialComposition + "',");
                    sb.Append("VarsanyBag='" + usermodel.VarsanyBag + "',Colour='" + usermodel.Colour + "',ProductWeight='" + usermodel.ProductWeight + "',");
                    sb.Append("BoxSize='" + usermodel.BoxSize + "',BoxQuenitity='" + usermodel.BoxQuantity + "',Manufactuer='" + usermodel.Manufacturer + "',");
                    sb.Append("MailingBag='" + usermodel.MaillingBag + "',Size='" + usermodel.Size + "',ProductCost='" + usermodel.ProductCost + "',");
                    sb.Append("ShippingCost='" + usermodel.Shippingncost + "',Comission='" + usermodel.Commision + "',Duty='" + usermodel.Duty + "',");
                    sb.Append("TotalCost='" + usermodel.TotalCost + "',ProductImage='" + imgfileName + "' where WarehouseSKU='" + WarehouseSKU+"'");
                    objDal.EXECUTE_DML(sb.ToString());
                    ViewBag.imageurl = imgfileName;
                    ViewBag.Title = "Update Catelogue Product";
                    ViewBag.Message = "Product Sucessfully Updated";
                    TempData["Message"] = "Update Product";
                }

                else
                {
                    if (usermodel.ProductImage != null)
                    {

                        string path = Server.MapPath("~/upload image/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        filePath = path + Path.GetFileName(usermodel.ProductImage.FileName);
                        string extension = Path.GetExtension(usermodel.ProductImage.FileName);
                        //usermodel.ProductImage.SaveAs(filePath);
                        string[] allowextension = new string[] { ".JPG", ".PNG", ".jpg", ".png", ".jpeg", ".gif", ".tiff", ".eps", ".ai", ".psd" };

                        string conString = string.Empty;
                        if (!allowextension.Contains(extension))
                        {
                            ViewBag.color = "red";
                            ViewBag.ErrorMessage = "Image extension not allowd";
                        }
                        sb.Append("select count(WarehouseSKU) from tblCatelogue where WarehouseSKU='" + usermodel.WarehouseSKU + "'");
                        int Count1 = Convert.ToInt32(objDal.Get_SingleValue(sb.ToString()));
                        if (Count1 > 0)
                        {
                            // Error message in red color
                            ViewBag.color = "red";
                            ViewBag.ErrorMessage = " Duplicate Warehouse SKU";
                        }
                        else
                        {
                            imgfileName = Guid.NewGuid().ToString() + Path.GetExtension(usermodel.ProductImage.FileName);

                            string Filepath = Path.Combine(path, imgfileName);
                            usermodel.ProductImage.SaveAs(Filepath);
                            sb = new StringBuilder();
                            //create new sqlcommand and command used to data insert query to get data from model tp database 
                            sb.Append("insert into tblCatelogue(Product_Name,WarehouseSKU,Description,HSCode,Width,Length,Height,MaterialComposition,");
                            sb.Append("VarsanyBag,Colour,ProductWeight,BoxSize,BoxQuenitity,Manufactuer,MailingBag,Size,ProductCost,ShippingCost,Comission,Duty,TotalCost,ProductImage)values ");
                            sb.Append("('" + usermodel.ProductName + "','" + usermodel.WarehouseSKU + "','" + usermodel.Description + "',");
                            sb.Append("'" + usermodel.HSCode + "','" + usermodel.Width + "','" + usermodel.Length + "','" + usermodel.Height + "',");
                            sb.Append("'" + usermodel.MaterialComposition + "','" + usermodel.VarsanyBag + "','" + usermodel.Colour + "','" + usermodel.ProductWeight + "',");
                            sb.Append("'" + usermodel.BoxSize + "','" + usermodel.BoxQuantity + "','" + usermodel.Manufacturer + "',");
                            sb.Append("'" + usermodel.MaillingBag + "','" + usermodel.Size + "','" + usermodel.ProductCost + "',");
                            sb.Append("'" + usermodel.Shippingncost + "','" + usermodel.Commision + "','" + usermodel.Duty + "','" + usermodel.TotalCost + "',");
                            sb.Append("'" + imgfileName + "')");
                            //EXECUTE_DML function call from dal class to excute sql query
                            objDal.EXECUTE_DML(sb.ToString());
                            ViewBag.imageurl = imgfileName;
                            //for message
                            ViewBag.Message = "Product Sucessfully Inserted";
                            ViewBag.color = "Green";
                            return View();
                        }

                    }
                }
                
            }

            return View();

        }
        [HttpGet]
        public JsonResult Removeimage(string warehouseSku)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                objDal=new Dal();
                sb.Append ("select ProductImage from tblCatelogue where WarehouseSKU='" + warehouseSku + "'");
                string image = objDal.Get_SingleValue(sb.ToString());
                if (!string.IsNullOrEmpty(image))
                {
                    // Set file path with directory
                    string filePath1 = Server.MapPath("~/upload image/" + image);
                    FileInfo files = new FileInfo(filePath1);
                    if (files.Exists)
                    {
                        files.Delete();
                    }
                    sb.Clear();
                    sb.Append("update tblCatelogue set ProductImage=null where WarehouseSKU='"+warehouseSku+"'");
                    objDal.EXECUTE_DML(sb.ToString());
                }
                return Json(new { iserror = false, result = 1, message = "File successfully deleted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { iserror = true, result = 1, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult cateloguepage(int? inputPageNo)
        {
            CatelogueModel Catelogue = new CatelogueModel();
            string imageurl = string.Empty;
            objDal = new Dal();
            sb = new StringBuilder();
            //vaiable decleration of pagenumber,pagesize,offset with assigned its value
            
            int pageNumber = 1;
            int pageSize = 3;
            int offSet = 0;
            int maxPages = 3;


            //vaiable decleration of tempdata
            //if tempdata is not null then retrive userlist to view
            var tempdata = TempData["Model"];
            if (tempdata != null)
            {
                return View(tempdata);
            }
            sb.Append("select count(*) from tblCatelogue ");
            double count = Convert.ToInt32(objDal.Get_SingleValue(sb.ToString()));
            /*count total page from total column divided by pagesize and using math.ceiling()for convert decimal value
           to nearest whole number*/
            double Totalpage = Math.Ceiling(count / pageSize);
            //store totalpage to model pagecount
            Catelogue.PagedList = Totalpage;
            // if inputPageNo is grater or equalto then pass inputpageNo value to pagenumber
            //pagenumber minus 1 into pagesize is offset
            if (inputPageNo <= Totalpage)
            {
                pageNumber = inputPageNo.Value;
                offSet = pageSize * (pageNumber - 1);
                Catelogue.pageNumber = pageNumber;
            }
            int startPage, endPage;
            if (Totalpage <= maxPages)
            {
                // total pages less than max so show all pages
                Catelogue.startPage = 1;
                Catelogue.endPage = (int)Totalpage;
            }
            else
            {
                // total pages more than max so calculate start and end pages
                var maxPagesBeforeCurrentPage = (int)Math.Floor((decimal)maxPages / (decimal)2);
                var maxPagesAfterCurrentPage = (int)Math.Ceiling((decimal)maxPages / (decimal)2) - 1;
                if (pageNumber <= maxPagesBeforeCurrentPage)
                {
                    // current page near the start
                    Catelogue.startPage = 1;
                    Catelogue.endPage = maxPages;
                }
                else if (pageNumber + maxPagesAfterCurrentPage >= Totalpage)
                {
                    // current page near the end
                    Catelogue.startPage = (int)(Totalpage - maxPages + 1);
                    Catelogue.endPage = (int)Totalpage;
                    
                }
                else
                {
                    // current page somewhere in the middle
                    Catelogue.startPage = pageNumber - maxPagesBeforeCurrentPage;
                    Catelogue.endPage = pageNumber + maxPagesAfterCurrentPage;
                }
            }
            
        
            Catelogue.pageNumber = pageNumber;
            sb=new StringBuilder();
            sb.Append("select Product_Name,TotalCost,ProductImage from tblCatelogue ");
            sb.Append($"ORDER BY Product_Name OFFSET {offSet} ROWS FETCH NEXT {pageSize} ROWS ONLY");
            //call get_singlevalue()to get single value
            dt =objDal.GetDataTable(sb.ToString());
            List<CatelogueModel>catelogues= new List<CatelogueModel>();
            if (dt.Rows.Count > 0)
            {
                //code to be executed repeatedly util row i less then 0
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CatelogueModel catelogue =new CatelogueModel();
                    catelogue.ProductName = dt.Rows[i]["Product_Name"].ToString();
                    catelogue.TotalCost = Convert.ToDecimal(dt.Rows[i]["TotalCost"].ToString());
                    catelogue.FileImage = catelogue.FileImage = dt.Rows[i]["ProductImage"].ToString();
                    catelogues.Add(catelogue);
                }

            }

            Catelogue.productList = catelogues;
             return View(Catelogue); 
        }


    }
}