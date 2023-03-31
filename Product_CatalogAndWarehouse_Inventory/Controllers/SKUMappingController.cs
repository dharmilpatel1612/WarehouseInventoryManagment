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
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Diagnostics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Security.Cryptography;
using System.Collections;
using System.ComponentModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class SKUMappingController : Controller
    {
        Dal obj_dal = new Dal();
        DataTable dt_Excel = new DataTable();
        DataTable dt_MappingSKUList = new DataTable();
        DataTable dt_RowExpansion = new DataTable();
        StringBuilder sb = new StringBuilder();

        public List<SKUMapping> mappingSKU()      
        {
            List<SKUMapping> SKUList = new List<SKUMapping>();
            // Used to check row count grater than 0
            if (dt_MappingSKUList.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dt_MappingSKUList.Rows.Count; i++)
                {
                    var SKUdata = new SKUMapping();
                    // Add serial number using row count in list
                    SKUdata.serialNo = i + 1;
                    // Get the value of WarehouseSKU
                    SKUdata.WarehouseSKU = dt_MappingSKUList.Rows[i]["Warehouse_SKU"].ToString();
                    // Get the value of MappingSKU
                    SKUdata.MappingSKU = dt_MappingSKUList.Rows[i]["MappingSKU"].ToString();
                    // Add SKUdata data into SKUList
                    SKUList.Add(SKUdata);
                }
            }
            return SKUList;
        }
        public List<SKUMapping> RowExpansionSKU()
        {
            List<SKUMapping> RowMappingSKU = new List<SKUMapping>();
            if (dt_RowExpansion.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dt_RowExpansion.Rows.Count; i++)
                {
                    var SKUValue = new SKUMapping();
                    // Get the value of MappingSKU
                    SKUValue.MappingSKU = dt_RowExpansion.Rows[i]["MappingSKU"].ToString();
                    // Add SKUValue data into RowMappingSKU
                    RowMappingSKU.Add(SKUValue);
                }
            }
            return RowMappingSKU;
        }

        public PartialViewResult GetMappingSku(string warehouseSKU)
        {
            SKUMapping sKUMapping = new SKUMapping();
            // if warehouseSKU value is not null then MappingSKU records add into model RowExpansionList.
            if (warehouseSKU != null)
            {
                // Applied SELECT Query to get data of MappingSKU from in database using WarehouseSKU.
                sb.Append("SELECT s.MappingSKU From tbl_SKUMapping s ");
                sb.Append("INNER JOIN tbl_WarehouseSKU w ON w.WarehouseSKU_Id = s.WarehouseSKU_Id ");
                sb.Append("WHERE w.Warehouse_SKU = '" + warehouseSKU + "'");
                // GET_DATATABLE function call from Dal class to fill the table data
                dt_RowExpansion = obj_dal.GET_DATATABLE(sb.ToString());
                // Function call
                sKUMapping.RowExpansionList = RowExpansionSKU();
            }
            return PartialView(sKUMapping);
        }

        // GET: SKUMapping
        public ActionResult SKUMapping(string warehouseSKU)
        {          
            SKUMapping sKUMapping = new SKUMapping();           
            // Applied SELECT Query to get data of MappingSKU from in database 
            sb.Clear();
            sb.Append("SELECT w.Warehouse_SKU, STRING_AGG(s.MappingSKU, ', ') AS MappingSKU ");
            sb.Append("FROM tbl_WarehouseSKU w ");
            sb.Append("INNER JOIN tbl_SKUMapping s ON w.WarehouseSKU_Id = s.WarehouseSKU_Id ");
            sb.Append("GROUP BY w.WarehouseSKU_Id,w.Warehouse_SKU");
            // GET_DATATABLE function call from Dal class to fill the table data
            dt_MappingSKUList = obj_dal.GET_DATATABLE(sb.ToString());
            // Function call
            sKUMapping.mappingSKUList = mappingSKU();           
            return View(sKUMapping);
        }

        [HttpPost]
        public ActionResult SKUMapping(SKUMapping sKUMapping)
        {
            try
            {
                /* if extensions are .xls and .xlsx then call importFile()
                otherwise return view */
                string extention = Path.GetExtension(sKUMapping.fileupload.FileName);
                if (extention != ".xls" & extention != ".xlsx")
                {
                    return View();
                }
                // Set directory path
                string path = Server.MapPath("~/Uploads/");
                // if directory is alredy exists then remove it with all file and create new directory
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                Directory.CreateDirectory(path);
                // For file path
                string filePath = path + Path.GetFileName(sKUMapping.fileupload.FileName);
                // For File extension
                string extension = Path.GetExtension(sKUMapping.fileupload.FileName);
                // Save as file in given file path
                sKUMapping.fileupload.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                dt_Excel = new DataTable();
                conString = string.Format(conString, filePath);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;
                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt_Excel);
                            connExcel.Close();
                        }
                    }
                }
                // Validation for Warehouse_SKU column
                if (dt_Excel.Columns.Count > 0 && dt_Excel.Columns[0].ColumnName == "Warehouse_SKU")
                {
                    sb.Append("DELETE from tbl_WarehouseSKU");
                    obj_dal.EXECUTE_DML(sb.ToString());
                    // BulkInsert for Warehousesku
                    obj_dal.BulkInsertWarehouse(dt_Excel, "tbl_WarehouseSKU");
                    sb.Clear();
                    sb.Append("DELETE From tbl_WarehouseSKU WHERE Warehouse_SKU Is Null");
                    obj_dal.EXECUTE_DML(sb.ToString());
                    DataTable dt_WarehouseSKU = new DataTable();
                    DataTable dt_MappingSKU = new DataTable();
                    // Add column in dt_MappingSKU table
                    dt_MappingSKU.Columns.Add("WarehouseSKU_Id", typeof(Guid));
                    dt_MappingSKU.Columns.Add("mapping_SKU");
                    sb.Clear();
                    // Applied select query to get data of tbl_WarehouseSKU from databse
                    sb.Append("select * from tbl_WarehouseSKU");
                    dt_WarehouseSKU = obj_dal.GET_DATATABLE(sb.ToString());
                    // loop to be executed repeatedly until row count less than 0
                    for (int i = 0; i < dt_WarehouseSKU.Rows.Count; i++)
                    {
                        // Get WarehouseSKU_Id store into var id
                        var id = dt_WarehouseSKU.Rows[i]["WarehouseSKU_Id"].ToString();
                        // Get Warehouse_SKU and store into variable
                        var SelectedWarehouseSKU = dt_Excel.Select($"Warehouse_SKU='{dt_WarehouseSKU.Rows[i]["Warehouse_SKU"].ToString()}'");
                        /* If statement used to check warehouse_SKU length is 0 or not
                         * if length greather than 0 then add WarehouseSKU_Id,mapping_SKU into dt_MappingSKU table.
                         * ItemArray.Skip(1) - used to skip first row
                        */
                        if (SelectedWarehouseSKU.Length > 0)
                        {
                            foreach (var MappingSKU in SelectedWarehouseSKU[0].ItemArray.Skip(1))
                            {
                                if (!string.IsNullOrEmpty(MappingSKU.ToString()))
                                {
                                    DataRow dr = dt_MappingSKU.NewRow();
                                    dr["WarehouseSKU_Id"] = id.ToString();
                                    dr["mapping_SKU"] = MappingSKU.ToString();
                                    dt_MappingSKU.Rows.Add(dr);
                                }
                            }
                        }
                    }
                    // BulkInsert for SKUMapping
                    obj_dal.BulkInsertMappingSKU(dt_MappingSKU, "tbl_SKUMapping");
                    return RedirectToAction("SKUMapping");
                }
                else
                {
                    // Error message in red color
                    ViewBag.color = "Red";
                    ViewBag.Message = "Invalid Column Name";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View(sKUMapping);
        }       
    }
}


