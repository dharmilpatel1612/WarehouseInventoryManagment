using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace Product_catalog_and_Warehouse_inventory1
{
    public class Dal
    {
        public SqlConnection cn;
        public SqlCommand cmd;
        public SqlDataAdapter da;

        public Dal()
        {
            cn = new SqlConnection(ConfigurationManager.ConnectionStrings["db-registration"].ToString());
        }

        // for execute dml statement
        public void EXECUTE_DML(string SQLQuery)
        {
            cmd = new SqlCommand();
            cmd.Connection = cn;
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            cmd.CommandText = SQLQuery;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();
        }

        // for Get single value
        public string Get_SingleValue(string SQLQuery)
        {
            string retval = "";
            cmd = new SqlCommand();
            cmd.Connection = cn;
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            cmd.CommandText = SQLQuery;
            if (cmd.ExecuteScalar() == null)
            {
                return retval;
            }
            retval = cmd.ExecuteScalar().ToString().Trim();
            cmd.Dispose();
            cn.Close();
            return retval;
        }

        public DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand(query, cn);
            cmd.CommandTimeout = 1000;
            da = new SqlDataAdapter(cmd);
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            da.Fill(dt);
            da.Dispose();
            cn.Close();
            return dt;
        }
        //for bulkwarehouse data
        public void BulkWareHouse(DataTable dt_temp,string TableName)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(cn);

            //set the timeout
            sqlBulkCopy.BulkCopyTimeout = 700;

            //set the database table name
            sqlBulkCopy.DestinationTableName= TableName;
            sqlBulkCopy.ColumnMappings.Add("Warehouse_SKU", "WareHouseSKU");
            
            if(cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            cn.Close();

        }
        //for BulkMappingSKU data
        public void BulkMappingSKU(DataTable dt_temp, string TableName)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(cn);

            //set the timeout
            sqlBulkCopy.BulkCopyTimeout = 700;

            //set the database table name
            sqlBulkCopy.DestinationTableName = TableName;
            sqlBulkCopy.ColumnMappings.Add("IdWareHouseSKU", "IdWareHouseSKU");
            sqlBulkCopy.ColumnMappings.Add("MappingSKU", "MappingSKU");
            sqlBulkCopy.ColumnMappings.Add("DateAdd", "DateAdd");


            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            cn.Close();

        }
    }
}
 
