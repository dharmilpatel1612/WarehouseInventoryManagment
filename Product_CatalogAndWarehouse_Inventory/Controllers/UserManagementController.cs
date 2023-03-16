using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class UserManagementController : Controller
    {
        StringBuilder sb;
        Dal obj_dal;
        DataTable dtuser;

        // Get: Registration
        public ActionResult Registration()
        {
            UserModel model = new UserModel();
            // Variable declaration if Tempdata is not null then pass the stored value in view
            var tempdata = TempData["Model"];
            if (tempdata != null)
            {
                return View(tempdata);
            }
            // Function call
            model.Cities = Getcitydatalist();
            // Onclick button text change
            //TempData["Message"] = "Submit";
            return View(model);

        }
        //public ActionResult UserDataInsert()
        //{

        //}

        public List<SelectListItem> Getcitydatalist()
        {
            // Dal Function call from Dal class
            obj_dal = new Dal();
            dtuser = new DataTable();
            // Applied SELECT Query to get data of city table from database
            sb = new StringBuilder();
            //sb.Append("SELECT * FROM tbl_City");
            sb.Append("SELECT City_Id, City_Name FROM Table_City");
            // GET_DATATABLE function call from Dal class to fill the table data
            dtuser = obj_dal.GET_DATATABLE(sb.ToString());
            List<SelectListItem> cityList = new List<SelectListItem>();
            SelectListItem city;
            // Used to check row count grater than 0
            if (dtuser.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dtuser.Rows.Count; i++)
                {
                    city = new SelectListItem();
                    // get the value of City_ID
                    city.Value = dtuser.Rows[i]["City_Id"].ToString();
                    // Get the value of City_Name
                    city.Text = dtuser.Rows[i]["City_Name"].ToString();
                    // Add City_ID and City_Name into city
                    cityList.Add(city);
                }
            }
            return cityList;
        }

        /* POST: UserDataInsert
           Used to handle the data passed between the database and the user interface (UI).*/
        [HttpPost]
        public ActionResult Registration(UserModel userModel, int? ID)
        {
            obj_dal = new Dal();
            sb = new StringBuilder();
            // If statement condition used for Update data and else statement used for Insert data
            if (ID != null)
            {
                // Used to Applied SELECT Query to count Email data from database using model
                sb.Append("SELECT COUNT(Email) FROM Table_Reg WHERE Email = '" + userModel.Email + "' and Reg_Id != '" + ID + "'");
                /* Variable declaration and convert into int
                   Get_SingleValue function call from Dal class to return single value*/
                int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                // If Row count value is grater then 0 so Email id must be duplicate
                if (Count > 0)
                {
                    // Error message in red color
                    ViewBag.color = "red";
                    ViewBag.Message = "Email ID Already exist";
                }
                else
                {
                    // Applied UPDATE Query to update row data in database using model
                    sb.Append("UPDATE Table_Reg SET Name = '" + userModel.Name + "',Email = '" + userModel.Email + "',");
                    sb.Append("Mobile_No = '" + userModel.Mobile_No + "',Gender = '" + userModel.Gender + "',City_ID = '" + userModel.city_Id + "', ");
                    sb.Append("Password = '" + userModel.Password + "' FROM Table_Reg  Where Reg_Id ='" + ID + "'");
                    // EXECUTE_DM function call from Dal class to execute sql query
                    obj_dal.EXECUTE_DML(sb.ToString());
                    TempData["message"] = "Data updated successfully!";
                    return RedirectToAction("UserList");
                }
                userModel = new UserModel();
                // Function call
                userModel.Cities = Getcitydatalist();
            }
            else
            {
                // Used to Applied SELECT Query to count Email data from database using model
                sb.Append("SELECT COUNT(Email) FROM Table_Reg WHERE Email = '" + userModel.Email + "'");
                /* Variable declaration and convert into int
                   Get_SingleValue function call from Dal class to return single value*/
                int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                // If Row count value is grater then 0 so Email id must be duplicate
                if (Count > 0)
                {
                    // Error message in red color
                    ViewBag.color = "red";
                    ViewBag.Message = "Email ID Already exist";
                }
                else
                {
                    sb.Clear();
                    // Used to applied INSERT Query to get data from model to database
                    sb.Append("INSERT INTO Table_Reg (Name, Email, Mobile_No, Gender, City_Id, Password)");
                    sb.Append("VALUES ('" + userModel.Name + "','" + userModel.Email + "','" + userModel.Mobile_No + "',");
                    sb.Append("'" + userModel.Gender + "','" + userModel.city_Id + "','" + userModel.Password + "')");
                    // EXECUTE_DM function call from Dal class to execute sql query
                    obj_dal.EXECUTE_DML(sb.ToString());
                    // Success Message in green color
                    TempData["message"] = "Data saved successfully!";
                    // Clear the model
                    ModelState.Clear();
                    return RedirectToAction("UserList");
                }
                userModel = new UserModel();
                // Function call
                userModel.Cities = Getcitydatalist();
            }
            return View(userModel);
        }

        public ActionResult UserList(UserModel userModel)
        {
            var session = Session["Username"];
            if (session == null)
            {
                return RedirectToAction("Login");
            }
            // Dal Function call from Dal class
            obj_dal = new Dal();
            sb = new StringBuilder();
            /* Applied SELECT Query to get data of user from registration table in database with pagination
               ORDER BY: Required, OFFSET:optional number of skipped rows , NEXT:required number of next rows */
            sb.Clear();
            sb.Append("SELECT r.Reg_Id,r.Name,r.Email,r.Mobile_No,r.Gender,c.City_Name ");
            sb.Append("FROM Table_Reg r ");
            sb.Append("INNER JOIN Table_City c ON r.City_Id = c.City_Id ");
            // GET_DATATABLE function call from Dal class to fill the table data
            dtuser = obj_dal.GET_DATATABLE(sb.ToString());
            // Function Call           
            userModel.userList = UserDataList();
            return View(userModel);
        }

        public List<UserModel> UserDataList()
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

        public ActionResult DeleteUserData(int Reg_id)
        {
            // Dal Function call from Dal class
            obj_dal = new Dal();
            // Applied DELETE Query to delete row record in list and table 
            sb = new StringBuilder();
            sb.Append("DELETE FROM Table_Reg where Reg_Id= '" + Reg_id + "'");
            // EXECUTE_DM function call from Dal class to execute sql query
            obj_dal.EXECUTE_DML(sb.ToString());
            return RedirectToAction("UserList");
        }

        public ActionResult EditUserData(int Reg_Id)
        {
            // Dal Function call from Dal class
            obj_dal = new Dal();
            UserModel userModel = new UserModel();
            // Applied SELECT Query to select row record in list and table for particular Reg_ID
            sb = new StringBuilder();
            sb.Append("SELECT r.Reg_Id,r.Name,r.Email,r.Mobile_No,r.Gender,c.City_Name,c.City_Id FROM Table_Reg r ");
            sb.Append("INNER JOIN Table_City c ON r.City_Id = c.City_Id WHERE r.Reg_Id = '" + Reg_Id + "'");
            dtuser = obj_dal.GET_DATATABLE(sb.ToString());
            // Used to check row count grater than 0
            if (dtuser.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dtuser.Rows.Count; i++)
                {
                    // userModel = new UserModel();
                    // Get the value of Name
                    userModel.Name = dtuser.Rows[i]["Name"].ToString();
                    // Get the value of Email ID
                    userModel.Email = dtuser.Rows[i]["Email"].ToString();
                    // Get the value of Mobile Number
                    userModel.Mobile_No = dtuser.Rows[i]["Mobile_No"].ToString();
                    // Get the value of Gender
                    userModel.Gender = dtuser.Rows[i]["Gender"].ToString();
                    // Get the value of City_Name
                    userModel.City = dtuser.Rows[i]["City_Name"].ToString();
                    // Get the value of City_Id
                    userModel.city_Id = Convert.ToInt32(dtuser.Rows[i]["City_Id"].ToString());
                }
            }
            // Function call
            userModel.Cities = Getcitydatalist();
            // Used Tempdata to keep record of particular row in userModel
            TempData["Model"] = userModel;
            // Onclick button text change 
            TempData["Message"] = "Update";
            return RedirectToAction("Registration", new { ID = Reg_Id });
        }

        public ActionResult Login()
        
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel userModel)
        {
            
             // Dal Function call from Dal class
                obj_dal = new Dal();
                sb = new StringBuilder();
                sb.Append("SELECT Count(*) FROM Table_Reg WHERE Email='" + userModel.Email + "' AND Password='" + userModel.Password + "'");
                int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
                if (Count == 1)
                {
                    sb.Clear();
                    sb.Append("SELECT Name FROM Table_Reg WHERE Email='" + userModel.Email + "' AND Password='" + userModel.Password + "'");
                    string Name = obj_dal.Get_SingleValue(sb.ToString());
                    Session["Username"] = Name;
                    return RedirectToAction("UserList");
                }
                else
                {
                    ViewBag.color = "red";
                    ViewBag.Message = " Wrong Email ID or Password";
                }
                return View(userModel);

            
        }
        public ActionResult Logout() 
        {
            var session = Session["Username"];
            if (session != null)
            {
                Session.Remove("Username");
            }
            return RedirectToAction("Login");
        }

    }
}