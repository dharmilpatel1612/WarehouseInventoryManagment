﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Product_CatalogAndWarehouse_Inventory
{
    public class Dal
    {
        public SqlConnection cn;
        public SqlCommand cmd;
        public SqlDataAdapter da;
        public DataTable dt;
        public StringBuilder sb;
        public Dal()
        {
            // For database connection
            cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["db-Registration"].ConnectionString;
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

        // for execute ddl statement
        public DataTable GET_DATATABLE(string SQLQuery)
        {
            dt = new DataTable();
            cmd = new SqlCommand(SQLQuery, cn);
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

        // For bulkwarehouse data
        public void BulkInsertWarehouse(DataTable dt_temp, string TableName)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(cn);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = TableName;
            sqlBulkCopy.ColumnMappings.Add("Warehouse_SKU", "Warehouse_SKU");
    
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp);
            cn.Close();
        }

        // For bulkmappingSKU data
        public void BulkInsertMappingSKU(DataTable dt_temp1, string TableName1)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(cn);

            // Set the timeout.
            sqlBulkCopy.BulkCopyTimeout = 700;

            //Set the database table name
            sqlBulkCopy.DestinationTableName = TableName1;
            sqlBulkCopy.ColumnMappings.Add("WarehouseSKU_Id", "WarehouseSKU_Id");
            sqlBulkCopy.ColumnMappings.Add("mapping_SKU", "MappingSKU");

            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            sqlBulkCopy.WriteToServer(dt_temp1);
            cn.Close();
        }

    }
}