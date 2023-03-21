using OfficeOpenXml;
using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Configuration;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class SKUMappingController : Controller
    {
        Dal obj_dal = new Dal();
        DataTable dt_SKUMapping = new DataTable();
        private bool isAllValid;

        // GET: SKUMapping
        public ActionResult SKUMapping()
        {
            return View();
        }

        //public async Task<List<SKUMapping>> importAsync()
        public List<SKUMapping> importFile(HttpPostedFileBase file)
        {
            obj_dal = new Dal();
            var list = new List<SKUMapping>();
            // Set directory path
            string path = Server.MapPath("~/Uploads/");
            // if directory is alredy exists then remove it with all file and create new directory
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);

            // For file path
            string filePath = path + Path.GetFileName(file.FileName);
            // Save as file in given file path
            file.SaveAs(filePath);

            using (var stream = new MemoryStream())
            {
                FileInfo existingFile = new FileInfo(filePath);
                //stream.Position = 0;
                //await file.CopyToAsync(stream);
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    // for file rowcount
                    var rowcount = worksheet.Dimension.Rows;
                    // code to be executed repeatedly until row count less than 2
                    for (int i = 2,count = 1; i <= rowcount; i++)
                    {
                        SKUMapping user = new SKUMapping();
                        // Get the value of Name from worksheet
                        user.Name = worksheet.Cells[i, 1].Value.ToString().Trim();
                        // Get the value of Email from worksheet
                        user.Email = worksheet.Cells[i, 2].Value.ToString().Trim();
                        // Check Name is Number and Email is valid or not.
                        if (!IsLetter(user.Name) | !IsEmail(user.Email))
                        {
                            // Error message in red color
                            ViewBag.color = "red";
                            ViewBag.Message = "Invalid " + (count) + " row in excel.";
                            isAllValid = false;
                            count++;
                        }
                        else
                        {
                            // Add user data into list
                            list.Add(user);
                        }             
                    }
                }
            }
            return list;           
        }

        private bool IsLetter(string value)
        {
            //Verify whether entered Name is only chracters or not.
            Regex regexLetter = new Regex(@"^[a-zA-Z]+$");
            return regexLetter.IsMatch(value);
        }
        private bool IsEmail(string value)
        {
            //Verify whether entered Email is in valid format or not using regular expression.
            Regex regexLetter = new Regex(@"^[A-Za-z0-9]+@[a-z]+.[a-z]{2,3}$");
            return regexLetter.IsMatch(value);
        }

        [HttpPost]
        public ActionResult SKUMapping(SKUMapping sKUMapping)
        {
            DataTable dt = new DataTable();
            /* if extensions are .xls and .xlsx then call importFile()
               otherwise return view */
            string extention = Path.GetExtension(sKUMapping.fileupload.FileName);
            if (extention != ".xls" & extention != ".xlsx")
            {
                return View();
            }
            // function call
            sKUMapping.List = importFile(sKUMapping.fileupload);
            // using SqlBulkCopy = new SqlBulkCopy(obj_dal.EXECUTE_DML();
            string conString = string.Empty;
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["db-Registration"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection =  cn;
            SqlBulkCopy bulkCopy = new SqlBulkCopy(cn);
            bulkCopy.DestinationTableName = "tbl_Registration";
           // bulkCopy.ColumnMappings.Add("Name", "Name");
            cn.Open();
            bulkCopy.WriteToServer((IDataReader)sKUMapping.List);
            return View(sKUMapping);
        }
    }
}