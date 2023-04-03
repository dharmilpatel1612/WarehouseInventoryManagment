using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.OleDb;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class SKUMappingController : Controller
    {
       
        StringBuilder sb;
        Dal objDal;
        DataTable dtMapping, dtExcelData, dtMappingSKU;
        DataTable mappingSKU = new DataTable();
        

        public ActionResult SKUMapping(string warehouseSKU = "")
        {
            SKUMapping sKUMapping = new SKUMapping();
            sKUMapping.MappingList = MappingDataList();


            return View(sKUMapping);
        }
        
        [HttpPost]
        public ActionResult SKUMapping(SKUMapping sKUMapping)
        {
            try
            {
                string filePath = string.Empty;
                if (sKUMapping.Fileupload != null)
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //if extension are .xls and .xlsx then call importfile
                    filePath = path + Path.GetFileName(sKUMapping.Fileupload.FileName);
                    string extension = Path.GetExtension(sKUMapping.Fileupload.FileName);
                    sKUMapping.Fileupload.SaveAs(filePath);
                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls":
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx":
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }
                    dtExcelData = new DataTable();
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

                                connExcel.Open();
                                //read data from first sheet
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dtExcelData);
                                connExcel.Close();
                            }
                        }
                    }

                    bool isAllValid = true;
                    //for (int i = 0; i < dtExcelData.Columns.Count; i++)
                    //{
                    //    if (dtExcelData.Columns.Count!=2)
                    //    {
                    //        ViewBag.Message = "column missing " + (i + 1) + " in excel.";
                    //        isAllValid = false;
                    //        break;
                    //    }
                    //}
                    if (isAllValid)
                    {
                        // Dal Function call from Dal class
                        objDal = new Dal();
                        sb = new StringBuilder();
                        // Applied DELETE Query to delete row record in list and table 
                        sb.Append("DELETE  FROM tblWarehouseSKU");
                        // EXECUTE_DM function call from Dal class to execute sql query
                        objDal.EXECUTE_DML(sb.ToString());
                        objDal.BulkInsertWarehouse(dtExcelData, "tblWarehouseSKU");
                        // Dal Function call from Dal class
                        objDal = new Dal();
                        sb = new StringBuilder();
                        // Applied SELECT Query to get data of tblwarehousesku table from database
                        sb.Append("SELECT * FROM tblWarehouseSKU");
                        DataTable dtwarehouse = new DataTable();
                        // GET_DATATABLE function call from Dal class to fill the table data
                        dtwarehouse = objDal.GET_DATATABLE(sb.ToString());
                        dtMapping = new DataTable();
                        //columns add
                        dtMapping.Columns.Add("idWarehouseSKU", typeof(Guid));
                        dtMapping.Columns.Add("MappingSKU");
                        dtMapping.Columns.Add("DateAdd");
                        // code to be executed repeatedly until row count less than 0
                        for (int i = 0; i < dtwarehouse.Rows.Count; i++)
                        {
                            // Get the value of idwarehousesku
                            string warehousesku = dtwarehouse.Rows[i]["idWarehouseSKU"].ToString();
                            var selectedwarehouse = dtExcelData.Select($"Warehouse_SKU='{dtwarehouse.Rows[i]["WarehouseSKU"].ToString()}'");
                            if (selectedwarehouse.Length > 0)
                            {
                                foreach (DataRow dr in selectedwarehouse)
                                {
                                    foreach (var sku in dr.ItemArray.Skip(1))
                                    {
                                        if (!string.IsNullOrEmpty(sku.ToString()))
                                        {
                                            DataRow dr1 = dtMapping.NewRow();
                                            // Get the value of idwarehousesku
                                            dr1["idWarehouseSKU"] = dtwarehouse.Rows[i]["idWarehouseSKU"].ToString();
                                            // Get the value of mappingsku
                                            dr1["MappingSKU"] = sku.ToString();
                                            // Get the value of dateadd
                                            dr1["DateAdd"] = DateTime.Now.ToString();
                                            dtMapping.Rows.Add(dr1);
                                        }

                                    }
                                }
                            }
                        }
                        //
                        objDal.BulkInsertMapping(dtMapping, "tblMappingSKU");
                        //function call 
                        sKUMapping.MappingList = MappingDataList();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.color = "red";
                ViewBag.Message = ex.Message;
            }
           
            return View(sKUMapping);

        }
        
        public PartialViewResult GetMappingSku(string idWarehouseSKU)
        {
            MappingSkuModel model = new MappingSkuModel();
            if (!string.IsNullOrEmpty(idWarehouseSKU))
            {
                objDal = new Dal();
                sb = new StringBuilder();
                sb.Clear();
                //select ni query no use kari ne warehousesku and mappingsku data display 
                sb.Append("SELECT MappingSKU FROM tblMappingSKU WHERE idWarehouseSKU='" + idWarehouseSKU + "' ");
                
                dtMappingSKU = new DataTable();
                // GET_DATATABLE function call from Dal class to fill the table data
                dtMappingSKU = objDal.GET_DATATABLE(sb.ToString());
                List<MappingSkuModel> mappingSkuList = new List<MappingSkuModel>();
                // Used to check row count grater than 0
                if (dtMappingSKU.Rows.Count > 0)
                {
                    // code to be executed repeatedly until row count less than 0
                    for (int i = 0; i < dtMappingSKU.Rows.Count; i++)
                    {
                        //var mappingSku = new MappingSkuModel();
                        //mappingSku.MappingSku = dtMappingSKU.Rows[i]["MappingSKU"].ToString();
                        //mappingSkuList.Add(mappingSku);

                        mappingSkuList.Add(new MappingSkuModel()
                        {
                            MappingSku = dtMappingSKU.Rows[i]["MappingSKU"].ToString()
                        });
                    }


                    model.MappingSkuList = mappingSkuList;

                }
            }
            return PartialView(model);
        }

        public List<SKUMapping> MappingDataList()
        {
            // Dal Function call from Dal class
            objDal = new Dal();
            sb = new StringBuilder();
            sb.Clear();
            //select ni query no use kari ne warehousesku and mappingsku data display 
            sb.Append("SELECT w.WarehouseSKU, w.idWarehouseSKU,STRING_AGG(m.MappingSKU,',')as MappingSKU ");
            sb.Append("FROM tblWarehouseSKU w ");
            sb.Append("INNER JOIN tblMappingSKU m on w.idWarehouseSKU=m.idWarehouseSKU ");
            sb.Append("GROUP BY w.idWarehouseSKU,w.WarehouseSKU ");
            DataTable SKUMapping = new DataTable();
            // GET_DATATABLE function call from Dal class to fill the table data
            SKUMapping = objDal.GET_DATATABLE(sb.ToString());
            List<SKUMapping> mappingdataList = new List<SKUMapping>();
            // Used to check row count grater than 0
            if (SKUMapping.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < SKUMapping.Rows.Count; i++)
                {
                    var user = new SKUMapping();
                    // Get the value of warehousesku
                    user.WarehouseSKU = SKUMapping.Rows[i]["WarehouseSKU"].ToString();
                    // Get the value of mappingsku
                    user.MappingSKU = SKUMapping.Rows[i]["MappingSKU"].ToString();
                    // Add user data into mappingdataList
                    user.idWarehouseSKU = SKUMapping.Rows[i]["idWarehouseSKU"].ToString();
                    mappingdataList.Add(user);
                }

            }

            return mappingdataList;
        }


    }
}


