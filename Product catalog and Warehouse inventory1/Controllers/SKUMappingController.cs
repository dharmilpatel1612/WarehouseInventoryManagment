using Product_catalog_and_Warehouse_inventory1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Data.OleDb;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Product_catalog_and_Warehouse_inventory1.Controllers
{
    public class SKUmappingController : Controller
    {
        //object 
        SqlCommand cmd;
        StringBuilder sb;
        Dal objDal;
        DataTable dtmappingSKU, dtExcelData,dtrowSKU;
        private object stream;
        private string List;

       
        public List<SKUMapping> MappingList()
        {
            objDal = new Dal();
            sb = new StringBuilder();
            sb.Clear();
            sb.Append("select w.WareHouseSKU,STRING_AGG(m.MappingSKU,',')as MappingSKU ");
            sb.Append("from tblwarehouseSKU w ");
            sb.Append("inner join tblmappingSKU m on w.IdWareHouseSKU= m.IdWareHouseSKU ");
            sb.Append("Group BY w.IdWareHouseSKU,w.WareHouseSKU ");
            DataTable SKUMapping = new DataTable();
            SKUMapping = objDal.GetDataTable(sb.ToString());
            List<SKUMapping> mappinguserList = new List<SKUMapping>();
            if (SKUMapping.Rows.Count > 0)
            {
                for (int i = 0; i < SKUMapping.Rows.Count; i++)
                {
                    var user = new SKUMapping();
                    user.WareHouseSKU = SKUMapping.Rows[i]["WareHouseSKU"].ToString();
                    user.MappingSKU = SKUMapping.Rows[i]["MappingSKU"].ToString();
                    //add user data into user
                    mappinguserList.Add(user);
                }
            }
            return mappinguserList;

        }
        // GET: SKUmapping
        public ActionResult SKUmapping(string WareHouseSKU)
        {
            SKUMapping skumapping = new SKUMapping();
            skumapping.MappingList = MappingList();
            if(!string.IsNullOrEmpty(WareHouseSKU))
            {
                sb = new StringBuilder();
                sb.Clear();
                sb.Append("select m.MappingSKU from tblmappingSKU m ");
                sb.Append("inner join tblwarehouseSKU w on w.IdWareHouseSKU=m.IdWareHouseSKU ");
                sb.Append("where w.WareHouseSKU='" + WareHouseSKU + "'");
                DataTable Rowmappingsku = new DataTable();
                Rowmappingsku = objDal.GetDataTable(sb.ToString());
                List<SKUMapping> mappinguserList = new List<SKUMapping>();
                if (Rowmappingsku.Rows.Count > 0)
                {
                    for (int i = 0; i < Rowmappingsku.Rows.Count; i++)
                    {
                        var user = new SKUMapping();
                        user.MappingSKU = Rowmappingsku.Rows[i]["MappingSKU"].ToString();
                        mappinguserList.Add(user);
                    }
                    skumapping.RowMappingSKU = mappinguserList;
                }
            }
            
            return View(skumapping);
        }

        [HttpPost]
        public ActionResult SKUmapping(SKUMapping skuMapping)
        {
            string filePath = string.Empty;
            if (skuMapping.Uploadfile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(skuMapping.Uploadfile.FileName);
                string extension = Path.GetExtension(skuMapping.Uploadfile.FileName);
                skuMapping.Uploadfile.SaveAs(filePath);

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
                            DataTable dtExcelSchema = new DataTable();
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
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
                //    if (dtExcelData.Columns.Count != 2)
                //    {
                //        ViewBag.Message = "column missing " + (i + 1) + " in excel.";
                //        isAllValid = false;
                //        break;
                //    }                 
                //}

                if (isAllValid)
                {
                    objDal = new Dal();
                    sb = new StringBuilder();
                    sb.Append("delete from tblwarehouseSKU");
                    objDal.EXECUTE_DML(sb.ToString());
                    //conString = ConfigurationManager.ConnectionStrings["db-registration"].ConnectionString;
                    objDal.BulkWareHouse(dtExcelData, "tblwarehouseSKU");
                    //using (SqlConnection con = new SqlConnection(conString))
                    //{
                    //    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    //    {

                    //        //Set the database table name.
                    //        sqlBulkCopy.DestinationTableName = "tblwarehouseSKU";

                    //        //[OPTIONAL]: Map the Excel columns with that of the database table
                    //        sqlBulkCopy.ColumnMappings.Add("Warehouse_SKU", "WareHouseSKU");
                    //        con.Open();
                    //        sqlBulkCopy.WriteToServer(dtExcelData);
                    //        con.Close();
                    //    }
                    //}
                    objDal = new Dal();
                    sb = new StringBuilder();
                    sb.Append("select * from tblwarehouseSKU");
                    DataTable dtwarehouse = new DataTable();
                    dtwarehouse = objDal.GetDataTable(sb.ToString());
                    DataTable dtmappingSKU = new DataTable();
                    dtmappingSKU.Columns.Add("IdWareHouseSKU", typeof(Guid));
                    dtmappingSKU.Columns.Add("MappingSKU");
                    dtmappingSKU.Columns.Add("DateAdd");

                    for (int i = 0; i < dtwarehouse.Rows.Count; i++)
                    {
                        string warewhouse = dtwarehouse.Rows[i]["IdWareHouseSKU"].ToString();
                        var selectedwarehouse = dtExcelData.Select($"Warehouse_SKU = '{dtwarehouse.Rows[i]["WareHouseSKU"].ToString()}'");
                        if (selectedwarehouse.Length > 0)
                        {
                            foreach (var SKU in selectedwarehouse[0].ItemArray.Skip(1))
                            {
                                if (!string.IsNullOrEmpty(SKU.ToString()))
                                {
                                    DataRow dr = dtmappingSKU.NewRow();
                                    dr["IdWareHouseSKU"] = dtwarehouse.Rows[i]["IdWareHouseSKU"].ToString();
                                    dr["MappingSKU"] = SKU.ToString();
                                    dr["DateAdd"] = DateTime.Now.ToString();
                                    dtmappingSKU.Rows.Add(dr);

                                }
                            }
                        }
                    }
                    objDal.BulkMappingSKU(dtmappingSKU, "tblmappingSKU");
                    skuMapping.MappingList = MappingList();
                   
                }
                
            }
            return View(skuMapping);


        }
        [HttpGet]
        public ActionResult RowClickMappingSKU(string WareHouseSKU)
        {
            SKUMapping skuMapping = new SKUMapping();
                objDal = new Dal();
                sb=new StringBuilder();
                sb.Clear();
                sb.Append("select m.MappingSKU from tblmappingSKU m ");
                sb.Append("inner join tblwarehouseSKU w on w.IdWareHouseSKU=m.IdWareHouseSKU ");
                sb.Append("where w.WareHouseSKU='" + WareHouseSKU + "'");
                DataTable Rowmappingsku = new DataTable();
                Rowmappingsku = objDal.GetDataTable(sb.ToString());
                List<SKUMapping> mappinguserList = new List<SKUMapping>();
            if (Rowmappingsku.Rows.Count > 0)
            {
                for (int i = 0; i < Rowmappingsku.Rows.Count; i++)
                {
                    var user = new SKUMapping();
                    user.MappingSKU = Rowmappingsku.Rows[i]["MappingSKU"].ToString();
                    mappinguserList.Add(user);
                }
                skuMapping.RowMappingSKU = mappinguserList;
            }
            return RedirectToAction("SKUMapping");
                          
       
        }
        
        
        
    }
}  
    



       
    



    