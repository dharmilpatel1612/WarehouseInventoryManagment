using Product_catalog_and_Warehouse_inventory1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Ajax.Utilities;
using System.Xml.Linq;

namespace Product_catalog_and_Warehouse_inventory1.Controllers
{

    public class UsermangementController : Controller
    {
        //object 
        SqlCommand cmd;
        StringBuilder sb;
        Dal objDal;
        DataTable dt;

        public List<Usermangement> UserListData()
        {
            List<Usermangement> userList = new List<Usermangement>();
            //Used to check row count grater then 0
            if (dt.Rows.Count > 0)
            {
                //code to be executed repeatedly util row i less then 0
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var user = new Usermangement();
                    //add serial no used count
                    user.SerialNo = i + 1;
                    //get the value of Registration_ID
                    user.Registration_ID = Convert.ToInt32(dt.Rows[i]["Registration_ID"].ToString());
                    //get the value of Name
                    user.Name = dt.Rows[i]["Name"].ToString();
                    //get the value of Email
                    user.Email = dt.Rows[i]["Email"].ToString();
                    //get the value of Mobilenum
                    user.Mobilenum = dt.Rows[i]["Mobile_No"].ToString();
                    //get the value of Gender
                    user.Gender = dt.Rows[i]["Gender"].ToString();
                    //get the value of CityId
                    user.City = dt.Rows[i]["CityName"].ToString();
                    //add user data into userlist
                    userList.Add(user);
                }
            }
            return userList;
        }
        //public List<Usermodel> datalist = new List<Usermodel>();
        // GET: validation
        public ActionResult Registration()
        {
            Usermangement model = new Usermangement();
            var tempdata = TempData["Model"];
            if (tempdata != null)
            {
                return View(tempdata);
            }
            //TempData["Message"] = "Submit";
            //function call
            model.Cities = getCityList();
            return View(model);

        }
        public List<SelectListItem> getCityList()
        {
            //Dal function call from dal class
            objDal = new Dal();
            DataTable dt_City = new DataTable();
            sb = new StringBuilder();
            //Applied select query to get data of city table from database
            sb.Append("Select * From tblCity");
            //GetDataTable function call from dal class to fill the table data 
            dt_City = objDal.GetDataTable(sb.ToString());
            List<SelectListItem> cityList = new List<SelectListItem>();
            SelectListItem city;
            //Used to check row count grater then 0
            if (dt_City.Rows.Count > 0)
            {
                //code to be executed repeatedly util row i less then count
                for (int i = 0; i < dt_City.Rows.Count; i++)
                {
                    city = new SelectListItem();
                    //get the value of City_ID
                    city.Value = dt_City.Rows[i]["CityId"].ToString();
                    //get the value of City_Name
                    city.Text = dt_City.Rows[i]["CityName"].ToString();
                    //Add City_ID and CIty_Name into city
                    cityList.Add(city);
                }
            }
            //connection colse
            return cityList;
        }
        [HttpPost]
        public ActionResult Registration(Usermangement usermodel, int? Id)
        {
            objDal = new Dal();
            if (Id != null)
            {
                sb = new StringBuilder();
                //Used to applied select query to count email data from database using model
                sb.Append("SELECT Count (*) ");
                sb.Append("FROM  tblUser ");
                sb.Append("WHERE Email='" + usermodel.Email + "'and Registration_ID!='" + Id + "'");
                //valiable declaration and convert inti int Get_SingleValue function call from dal to retrun singal value
                int count = Convert.ToInt32(objDal.Get_SingleValue(sb.ToString()));
                // If row count value is grater then 0 so email must be duplicate
                if (count > 0)
                {
                    //message print email already exist
                    ViewBag.Message = "Email Already Exits";
                    ViewBag.color = "red";
                }
                else
                {
                    sb = new StringBuilder();
                    //used to appiled update query and update data
                    sb.Append("UPDATE tblUser set Name='" + usermodel.Name + "',Email='" + usermodel.Email + "',");
                    sb.Append("Mobile_No='" + usermodel.Mobilenum + "',Gender='" + usermodel.Gender + "',CityId='" + usermodel.Cityid + "',");
                    sb.Append("Password='" + usermodel.Password + "' where Registration_ID='" + Id + "'");
                    objDal.EXECUTE_DML(sb.ToString());
                    //for message
                    TempData["Message"] = "Data Sucessfully Updated";
                    return RedirectToAction("Userlist");
                }
                usermodel = new Usermangement();
                //Display Cities count
                usermodel.Cities = getCityList();
            }
            else
            {
                sb = new StringBuilder();
                //Used to applied select query to count email data from database using model
                sb.Append("SELECT Count (*) ");
                sb.Append("FROM  tblUser ");
                sb.Append("WHERE Email='" + usermodel.Email + "'");
                //valiable declaration and convert inti int Get_SingleValue function call from dal to retrun singal value
                int count = Convert.ToInt32(objDal.Get_SingleValue(sb.ToString()));
                // If row count value is grater then 0 so email must be duplicate
                if (count > 0)
                {
                    //message print email already exist
                    ViewBag.Message = "Email Already Exits";
                    ViewBag.color = "red";
                }
                else
                {
                    //create new sqlcommand and command used to data insert query to get data from model tp database  
                    sb.Clear();
                    sb.Append("INSERT INTO  tblUser(Name,Email,Mobile_No,Gender,CityId,Password)");
                    sb.Append("VALUES ('" + usermodel.Name + "','" + usermodel.Email + "',");
                    sb.Append("'" + usermodel.Mobilenum + "'," + "'" + usermodel.Gender + "',");
                    sb.Append("'" + usermodel.Cityid + "','" + usermodel.Password + "')");
                    //EXECUTE_DML function call from dal class to excute sql query
                    objDal.EXECUTE_DML(sb.ToString());

                    //for message
                    TempData["Message"] = "Data Sucessfully Inserted";
                    ViewBag.color = "Green";
                    //clear the model
                    ModelState.Clear();
                    return RedirectToAction("UserList"/*,new {IsRecordInsertd= true}*/);
                    //return RedirectToAction("validationmodellist", "validation");
                }
                usermodel = new Usermangement();
                //Display Cities count
                usermodel.Cities = getCityList();
            }

            //output hold
            return View(usermodel);
        }
        public ActionResult UserList(Usermangement usermodel, int? inputPageNo, string columName = "Name", string ascorDesc = "Desc")
        {
            var session = Session["Username"];

            if (session == null)
            {
                return RedirectToAction("Login");
            }
            //Dal function call from dal class
            objDal = new Dal();
            sb = new StringBuilder();
            
            /*Applied select query to get data of user of registration  table from database with pagination
             order by:requied,OFFSET : optional number of skipped row,NEXT:requied number of next rows*/
            sb.Append("select u.Registration_ID, u.Name,u.Email,u.Mobile_No,u.Gender,c.CityName from tblUser u ");
            sb.Append("INNER JOIN tblCity c on u.CityId=c.CityId ");
            /* If statement used to check columnName is cityName or not
             if statment is true then CityName columns data is order by into asc and desc order
            otherwise tbluser table column name data is order by into asc and desc order*/           
            //GetDataTable function call from dal class to fill the table data 
            dt = objDal.GetDataTable(sb.ToString());
            //usermodel.validatiomodellist = userList;
            usermodel.Validationmodellist = UserListData();
            return View(usermodel);
        }

        public ActionResult DeleteUserData(int Registration_ID)
        {
            //dal function call from dal class
            objDal = new Dal();
            sb = new StringBuilder();
            //applied delete query to delete row record in list and table
            sb.Append("delete from tblUser where Registration_ID = '" + Registration_ID + "'");
            //GetDataTable function call from dal class to excute sql query
            objDal.EXECUTE_DML(sb.ToString());
            return RedirectToAction("Userlist");

        }
        public ActionResult EditUserData(int Registration_ID)
        {
            //Dal function call from dal class
            objDal = new Dal();
            Usermangement usermodel = new Usermangement();
            sb = new StringBuilder();
            sb.Append("select u.Registration_ID, u.Name,u.Email,u.Mobile_No,u.Gender,c.CityName,c.CityId from tblUser u ");
            sb.Append("INNER JOIN tblCity c on u.CityId=c.CityId where Registration_ID='" + Registration_ID + "'");
            objDal.EXECUTE_DML(sb.ToString());
            //GetDataTable function call from dal class to fill the table data 
            dt = objDal.GetDataTable(sb.ToString());
            //List<Usermodel> userList = new List<Usermodel>();
            //Used to check row count grater then 0
            if (dt.Rows.Count > 0)
            {
                //code to be executed repeatedly util row i less then 0
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    usermodel = new Usermangement();
                    //get the value of Name
                    usermodel.Name = dt.Rows[i]["Name"].ToString();
                    //get the value of Email
                    usermodel.Email = dt.Rows[i]["Email"].ToString();
                    //get the value of Mobilenum
                    usermodel.Mobilenum = dt.Rows[i]["Mobile_No"].ToString();
                    //get the value of Gender
                    usermodel.Gender = dt.Rows[i]["Gender"].ToString();
                    //get the value of CityId
                    usermodel.City = dt.Rows[i]["CityName"].ToString();
                    usermodel.Cityid = Convert.ToInt32(dt.Rows[i]["CityId"].ToString());
                    //add user data into userlist
                    //ViewBag.Cities = getCityList();
                    usermodel.Cities = getCityList();
                }
            }
            TempData["Model"] = usermodel;
            TempData["Message"] = "Update";
            return RedirectToAction("Registration", new { Id = Registration_ID });
        }
        [HttpPost]
        public ActionResult searchUserData(string search)
        {
            objDal = new Dal();
            Usermangement usermodel = new Usermangement();
            sb = new StringBuilder();
            //applied select query to get keyword search data of user record in database
            sb.Append("select u.Registration_ID, u.Name,u.Email,u.Mobile_No,u.Gender,c.CityName from tblUser u ");
            sb.Append("INNER JOIN tblCity c on u.CityId=c.CityId ");
            sb.Append("where CONCAT (Name,Email,Mobile_No,Gender,CityName) like '%" + search + "%'");
            dt = objDal.GetDataTable(sb.ToString());
            //created userlist object to store user search data
            usermodel.Validationmodellist = UserListData();
            TempData["Model"] = usermodel;
            return RedirectToAction("Userlist");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Usermangement usermodel)
        {
            
            
                objDal = new Dal();
                sb = new StringBuilder();
                sb.Append("select COUNT(*) from tblUser where Email= '" + usermodel.Email + "'and Password='" + usermodel.Password + "'");
                int count = Convert.ToInt32(objDal.Get_SingleValue(sb.ToString()));
                if (count == 1)
                {
                    sb.Clear();
                    sb.Append("select Name from tblUser where Email= '" + usermodel.Email + "'and Password='" + usermodel.Password + "'");
                    string Name = objDal.Get_SingleValue(sb.ToString());
                    Session["Username"] = Name;
                    //TempData["Message"] = "LoginPage Successfully";
                    return RedirectToAction("Userlist");
                }
                else
                {
                    ViewBag.color = "red";
                    ViewBag.Message = "Email Id OR Password Wrong";

                }
            
            return View(usermodel);
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
