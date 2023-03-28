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

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class SKUMappingController : Controller
    {
        SqlCommand cmd;
        StringBuilder sb;
        Dal objDal;
        DataTable dtMapping, dtExcelData;
        DataTable mappingSKU = new DataTable();
        private string value;

        public List<SKUMapping> MappingDataList()
        {

            objDal = new Dal();
            sb = new StringBuilder();
            sb.Clear();
            sb.Append("SELECT w.WarehouseSKU,m.MappingSKU ");
            sb.Append("FROM tblWarehouseSKU w ");
            sb.Append("INNER JOIN tblMappingSKU m on w.idWarehouseSKU=m.idWarehouseSKU ");
            DataTable SKUMapping = new DataTable();
            SKUMapping = objDal.GET_DATATABLE(sb.ToString());
            List<SKUMapping> mappingdataList = new List<SKUMapping>();
            if (SKUMapping.Rows.Count > 0)
            {
                for (int i = 0; i < SKUMapping.Rows.Count; i++)
                {
                    var user = new SKUMapping();
                    user.SerialNo = i + 1;
                    user.WarehouseSKU = SKUMapping.Rows[i]["WarehouseSKU"].ToString();
                    user.MappingSKU = SKUMapping.Rows[i]["MappingSKU"].ToString();
                    mappingdataList.Add(user);
                }

            }

            return mappingdataList;
        }

        public ActionResult Test()
        {
            Dal obj_dal = new Dal();
            sb = new StringBuilder();
            /* Applied SELECT Query to get data of user from registration table in database with pagination
               ORDER BY: Required, OFFSET:optional number of skipped rows , NEXT:required number of next rows */
            sb.Clear();
            sb.Append("SELECT r.Reg_Id,r.Name,r.Email,r.Mobile_No,r.Gender,c.City_Name ");
            sb.Append("FROM Table_Reg r ");
            sb.Append("INNER JOIN Table_City c ON r.City_Id = c.City_Id ");
            // GET_DATATABLE function call from Dal class to fill the table data
            DataTable dtuser = obj_dal.GET_DATATABLE(sb.ToString());
            // Function Call
            TestModel userModel=new TestModel();
            //userModel.userList = UserDataList(dtuser);
            return View(userModel);
        }

        public List<UserModel> UserDataList(DataTable dtuser)
        {
            List<UserModel> userdataList = new List<UserModel>();
            // Used to check row count grater than 0
            if (dtuser.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dtuser.Rows.Count; i++)
                {
                    var user = new UserModel();
                    // Add serial number using row count in list 
                    user.SerialNo = i + 1;
                    user.Reg_Id = Convert.ToInt32(dtuser.Rows[i]["Reg_Id"].ToString());
                    // Get the value of Name
                    user.Name = dtuser.Rows[i]["Name"].ToString();
                    // Get the value of Email ID
                    user.Email = dtuser.Rows[i]["Email"].ToString();
                    // Get the value of Mobile Number
                    user.Mobile_No = dtuser.Rows[i]["Mobile_No"].ToString();
                    // Get the value of Gender
                    user.Gender = dtuser.Rows[i]["Gender"].ToString();
                    // Get the value of City_Name
                    user.City = dtuser.Rows[i]["City_Name"].ToString();
                    // Add user data into userdataList
                    userdataList.Add(user);
                }
            }
            return userdataList;
        }

        // GET: SKUMapping
        public ActionResult SKUMapping()
        {
            SKUMapping sKUMapping=new SKUMapping();
            //sKUMapping.MappingList = MappingDataList();
            return View(sKUMapping);
        }

        [HttpPost]
        public ActionResult SKUMapping(SKUMapping sKUMapping)
        {
            string filePath= string.Empty;
            if (sKUMapping.Fileupload != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
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
                DataTable dtExcelData = new DataTable();
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
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dtExcelData);
                            connExcel.Close();
                            
                            //using (OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * From [ + sheetName + ]", connExcel))
                            //{ 
                            //    oda.Fill(dtExcelData);
                            //    for(int i=0;i<dtExcelData.Rows.Count;i++)
                            //    {
                            //        dt.Rows.Add(dtExcelData.Rows[i][0].ToString(), dtExcelData.Rows[i][1].ToString());
                            //    }
                            //}
                            //connExcel.Close();
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
                    objDal = new Dal();
                    sb = new StringBuilder();
                    sb.Append("DELETE  FROM tblWarehouseSKU");
                    objDal.EXECUTE_DML(sb.ToString());
                    objDal.BulkInsertWarehouse(dtExcelData, "tblWarehouseSKU");
                    //conString = ConfigurationManager.ConnectionStrings["db-Registration"].ConnectionString;
                    //using (SqlConnection con = new SqlConnection(conString))
                    //{
                    //    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    //    {
                    //        sqlBulkCopy.DestinationTableName = "tblWarehouseSKU";
                    //        sqlBulkCopy.ColumnMappings.Add("Warehouse_SKU", "WarehouseSKU");
                    //        con.Open();
                    //        sqlBulkCopy.WriteToServer(dtExcelData);
                    //        con.Close();
                    //    }
                    //}
                    objDal = new Dal();
                    sb = new StringBuilder();
                    sb.Append("SELECT * FROM tblWarehouseSKU");
                    DataTable dtwarehouse = new DataTable();
                    dtwarehouse=objDal.GET_DATATABLE(sb.ToString());
                    DataTable dtmapping = new DataTable();
                    dtmapping.Columns.Add("idWarehouseSKU",typeof(Guid));
                    dtmapping.Columns.Add("MappingSKU");
                    dtmapping.Columns.Add("DateAdd");
                    for (int i = 0; i < dtwarehouse.Rows.Count; i++)
                    { 
                        
                        string warehousesku = dtwarehouse.Rows[i]["idWarehouseSKU"].ToString();
                        var selectedwarehouse = dtExcelData.Select($"Warehouse_SKU='{dtwarehouse.Rows[i]["WarehouseSKU"].ToString()}'");
                        if(selectedwarehouse.Length>0)
                        {
                            foreach(DataRow dr in selectedwarehouse) 
                            {
                                foreach (var sku in dr.ItemArray.Skip(1))
                                {
                                    if (!string.IsNullOrEmpty(sku.ToString()))
                                    {
                                        DataRow dr1 = dtmapping.NewRow();
                                        dr1["idWarehouseSKU"] = dtwarehouse.Rows[i]["idWarehouseSKU"].ToString();
                                        dr1["MappingSKU"]= sku.ToString();
                                        dr1["DateAdd"] =DateTime.Now.ToString();
                                        dtmapping.Rows.Add(dr1);
                                    }
                                    
                                }
                            }
                        }
                    }
                    objDal.BulkInsertMapping(dtmapping, "tblMappingSKU");
                    //sKUMapping.MappingList = MappingDataList();  
                }
            }
            return View(sKUMapping);

        }
   

      
    }
}


