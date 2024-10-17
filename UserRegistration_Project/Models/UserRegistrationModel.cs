using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserRegistration_Project.Models
{
    public class UserRegistrationModel
    {
        public int Id { get; set; } 

       [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [StringLength(10, ErrorMessage = "Mobile number must be 10 digits long.")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
    }


}