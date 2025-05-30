﻿using Microsoft.AspNetCore.Identity;

// Make sure to use the EXACT same namespace as your project
namespace PokheraliDevelopers.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string ZipCode { get; set; }

        // Add SuccessfulOrderCount property
        public int SuccessfulOrderCount { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}