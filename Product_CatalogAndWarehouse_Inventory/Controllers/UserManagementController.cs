using Product_CatalogAndWarehouse_Inventory.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Product_CatalogAndWarehouse_Inventory.Controllers
{
    public class UserManagementController : Controller
    {
        StringBuilder sb;
        Dal obj_dal;
        DataTable dt;

        public List<UserModel> UserDataList()
        {
            List<UserModel> userdataList = new List<UserModel>();
            // Used to check row count grater than 0
            if (dt.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var user = new UserModel();
                    // Add serial number using row count in list 
                    user.SerialNo = i + 1;
                    user.registration_id = Convert.ToInt32(dt.Rows[i]["Registration_ID"].ToString());
                    // Get the value of Name
                    user.Name = dt.Rows[i]["Name"].ToString();
                    // Get the value of Email ID
                    user.Email = dt.Rows[i]["Email"].ToString();
                    // Get the value of Mobile Number
                    user.MobileNumber = dt.Rows[i]["Mobile_Number"].ToString();
                    // Get the value of Gender
                    user.Gender = dt.Rows[i]["Gender"].ToString();
                    // Get the value of City_Name
                    user.City = dt.Rows[i]["City_Name"].ToString();
                    // Add user data into userdataList
                    userdataList.Add(user);
                }
            }
            return userdataList;
        }

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
            TempData["Message"] = "Submit";
            return View(model);
        }

        public List<SelectListItem> Getcitydatalist()
        {
            // Dal Function call from Dal class
            obj_dal = new Dal();
            dt = new DataTable();
            // Applied SELECT Query to get data of city table from database
            sb = new StringBuilder();
            //sb.Append("SELECT * FROM tbl_City");
            sb.Append("SELECT City_ID, City_Name FROM tbl_City");
            // GET_DATATABLE function call from Dal class to fill the table data
            dt = obj_dal.GET_DATATABLE(sb.ToString());
            List<SelectListItem> cityList = new List<SelectListItem>();
            SelectListItem city;
            // Used to check row count grater than 0
            if (dt.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    city = new SelectListItem();
                    // get the value of City_ID
                    city.Value = dt.Rows[i]["City_ID"].ToString();
                    // Get the value of City_Name
                    city.Text = dt.Rows[i]["City_Name"].ToString();
                    // Add City_ID and City_Name into city
                    cityList.Add(city);
                }
            }
            return cityList;
        }

        [HttpPost]
        public ActionResult Registration(UserModel userModel, int? ID)
        {
            obj_dal = new Dal();
            sb = new StringBuilder();
            // If statement condition used for Update data and else statement used for Insert data
            if (ID != null)
            {
                // Used to Applied SELECT Query to count Email data from database using model
                sb.Append("SELECT COUNT(Email) FROM tbl_Registration WHERE Email = '" + userModel.Email + "' and Registration_ID != '" + ID + "'");
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
                    sb.Append("UPDATE tbl_Registration SET Name = '" + userModel.Name + "',Email = '" + userModel.Email + "',");
                    sb.Append("Mobile_Number = '" + userModel.MobileNumber + "',Gender = '" + userModel.Gender + "',City_ID = '" + userModel.city_Id + "', ");
                    sb.Append("Password = '" + userModel.Password + "' FROM tbl_Registration  Where Registration_ID ='" + ID + "'");
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
                sb.Append("SELECT COUNT(Email) FROM tbl_Registration WHERE Email = '" + userModel.Email + "'");
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
                    sb.Append("INSERT INTO tbl_Registration (Name, Email, Mobile_Number, Gender, City_ID, Password)");
                    sb.Append("VALUES ('" + userModel.Name + "','" + userModel.Email + "','" + userModel.MobileNumber + "',");
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
            // Stored seesion value in variable
            var session = Session["Username"];
            /* if statement is used check seesion value is null and 
             * if it's null then redirect login page */
            if (session == null)
            {
                return RedirectToAction("Login");
            }
            // Dal Function call from Dal class
            obj_dal = new Dal();
            sb = new StringBuilder();
            /* Variable declaration of tempdata
             * if tempdata is not null then retrive userList to view */
            var tempdata = TempData["Model"];
            if (tempdata != null)
            {
                return View(tempdata);
            }
            /* Applied SELECT Query to get data of user from registration table in database */
            sb.Clear();
            sb.Append("SELECT r.Registration_ID,r.Name,r.Email,r.Mobile_Number,r.Gender,c.City_Name ");
            sb.Append("FROM tbl_Registration r ");
            sb.Append("INNER JOIN tbl_City c ON r.City_ID = c.City_ID ");
            // GET_DATATABLE function call from Dal class to fill the table data
            dt = obj_dal.GET_DATATABLE(sb.ToString());
            // Function Call           
            userModel.userList = UserDataList();
            return View(userModel);
        }

        public ActionResult DeleteUserData(int registraion_id)
        {
            // Dal Function call from Dal class
            obj_dal = new Dal();
            // Applied DELETE Query to delete row record in list and table 
            sb = new StringBuilder();
            sb.Append("DELETE FROM tbl_Registration where Registration_ID= '" + registraion_id + "'");
            // EXECUTE_DM function call from Dal class to execute sql query
            obj_dal.EXECUTE_DML(sb.ToString());
            return RedirectToAction("UserList");
        }

        public ActionResult EditUserData(int registration_id)
        {
            // Dal Function call from Dal class
            obj_dal = new Dal();
            UserModel userModel = new UserModel();
            // Applied SELECT Query to select row record in list and table for particular Reg_ID
            sb = new StringBuilder();
            sb.Append("SELECT r.Registration_ID,r.Name,r.Email,r.Mobile_Number,r.Gender,c.City_Name,c.City_ID FROM tbl_Registration r ");
            sb.Append("INNER JOIN tbl_City c ON r.City_ID = c.City_ID WHERE r.Registration_ID = '" + registration_id + "'");
            dt = obj_dal.GET_DATATABLE(sb.ToString());
            // Used to check row count grater than 0
            if (dt.Rows.Count > 0)
            {
                // code to be executed repeatedly until row count less than 0
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    // userModel = new UserModel();
                    // Get the value of Name
                    userModel.Name = dt.Rows[i]["Name"].ToString();
                    // Get the value of Email ID
                    userModel.Email = dt.Rows[i]["Email"].ToString();
                    // Get the value of Mobile Number
                    userModel.MobileNumber = dt.Rows[i]["Mobile_Number"].ToString();
                    // Get the value of Gender
                    userModel.Gender = dt.Rows[i]["Gender"].ToString();
                    // Get the value of City_Name
                    userModel.City = dt.Rows[i]["City_Name"].ToString();
                    // Get the value of City_Id
                    userModel.city_Id = Convert.ToInt32(dt.Rows[i]["City_ID"].ToString());
                }
            }
            // Function call
            userModel.Cities = Getcitydatalist();
            // Used Tempdata to keep record of particular row in userModel
            TempData["Model"] = userModel;
            // Onclick button text change 
            TempData["Message"] = "Update";
            return RedirectToAction("Registration", new { ID = registration_id });
        }
        public ActionResult Login()
        {
            // Stored seesion value in variable
            var session = Session["Username"];
            /* if statement is used check seesion value is null or not and 
             * if it's not null then redirect UserList page and null then redirect Login page*/
            if (session != null)
            {
                return RedirectToAction("UserList");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel userModel)
        {
            obj_dal = new Dal();
            sb = new StringBuilder();
            sb.Append("SELECT count(*) FROM tbl_Registration where Email='" + userModel.Email + "' AND Password ='" + userModel.Password + "'");
            int Count = Convert.ToInt32(obj_dal.Get_SingleValue(sb.ToString()));
            if (Count == 1)
            {
                sb.Clear();
                sb.Append("SELECT Name FROM tbl_Registration where Email='" + userModel.Email + "'");
                string Username = obj_dal.Get_SingleValue(sb.ToString());
                // To keep username until session remove
                Session["Username"] = Username;
                return RedirectToAction("UserList");
            }
            else
            {
                // Error message in red color
                ViewBag.color = "red";
                ViewBag.Message = "Wrong Email ID or password";
            }
            return View(userModel);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var session = Session["Username"];
            // if statement used for check session is null or not
            if (session != null)
            {
                // Remove username when seesion closed 
                Session.Remove("Username");
            }
            return RedirectToAction("Login");
        }
    }
}