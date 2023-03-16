
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Product_catalog_and_Warehouse_inventory1.Models
{
    public class Usermangement
    {
        public int Registration_ID { get; set; }
        public int SerialNo { get; set; }
        [Required]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters Required"), MinLength(3, ErrorMessage = "Minimum length is 3")]
        [RegularExpression("^[A-z]+[a-z]$", ErrorMessage = "only characters are allowed")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter your email address")]
        [Display(Name = "Email address")]
        [RegularExpression("^[A-Za-z0-9]+@[a-z]+\\.[a-z]{2,3}$", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid phone number")]
        [Display(Name = "Mobilenum")]
        public string Mobilenum { get; set; }
        [Required]
        public string Gender { get; set; }
        public string City { get; set; }
        [Required]
        public int Cityid { get; set; }
        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        [Required]
        public List<Usermangement> Validationmodellist { get; set; }
        public List<Usermangement> usermodel { get; set; }
        public string search_keyword { get; set; }
        public double? PagedList { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@#!%*?&])[A-Za-z\d$@#!%*?&]{8,}$", ErrorMessage = "Min 8 char atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Char")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Confirmpassword")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@#!%*?&])[A-Za-z\d$@#!%*?&]{8,}$", ErrorMessage = "Min 8 char atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Char")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirmpassword doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}