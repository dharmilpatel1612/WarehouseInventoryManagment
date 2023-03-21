using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace Product_CatalogAndWarehouse_Inventory.Models
{
    public class UserModel
    {
        public int SerialNo { get; set; }        
        public int registration_id { get; set; }
        // Validation for Name (1.required 2.Length - max(50), min(3) 3. Only characters)
        [Required]
        [StringLength(50, ErrorMessage = "Maximum 50 Characters Required"), MinLength(3, ErrorMessage = "Minimum of 3 characters required")]
        [RegularExpression("^[A-z]+[a-z]$", ErrorMessage = "Only Chracters are allowed")]
        // Return the value or assign the value to Name
        public string Name { get; set; }
        // Validation for Email (1.required, 2.Email Format 
        [Required]
        [RegularExpression("^[A-Za-z0-9]+@[a-z]+\\.[a-z]{2,3}$", ErrorMessage = "Invalid Email")]
        // Return the value or assign the value to Email
        public string Email { get; set; }
        // Validation for Mobile Number (1.required, 2. Allowed only numbers and length must be 10)
        [Required]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Only numbers are allowed and Range must be 10")]
        [Display(Name = "Mobile Number")]
        // Return the value or assign the value to Mobile Number
        public string MobileNumber { get; set; }
        // Validation for Gender (1.required)
        [Required]
        // Return the value or assign the value to Gender
        public string Gender { get; set; }
        // Validation for city_Id (1.required)
        [Required]
        // Return the CityID or assign the ID of City
        public int city_Id { get; set; }
        // Return the name or assign the name of City       
        public string City { get; set; }
        // Validation for Password (1.required, 2.Password Format, 3.password dot format validation
        [Required(ErrorMessage = "Please enter Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@#!%*?&])[A-Za-z\d$@#!%*?&]{8,}$",
            ErrorMessage = "Must contain at least one number,uppercase,lowercase letter,special character and 8 or more characters")]
        [DataType(DataType.Password)]
        // Return the password
        public string Password { get; set; }
        // Validation for Password (1.required, 2.Compare password validation
        [Required(ErrorMessage = "Please enter Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        // Return the Confirm_Password
        public string Confirm_Password { get; set; }       
        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        public List<UserModel> userList { get; set; }
        public List<UserModel> EditList { get; set; }
        public double? Page_Count { get; set; }

    }
}