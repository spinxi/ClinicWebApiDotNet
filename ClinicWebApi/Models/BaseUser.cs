using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ClinicWebApi.Models
{
    public class BaseUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IdNumber { get; set; }
        public string? Role { get; set; }
        public byte[]? ProfileImage { get; set; }
        public bool? Pinned { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public BaseUser()
        {
            CreatedDate = DateTime.Now;
        }
    }

    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        public string? UserId { get; set; } // Foreign key with the same type as the primary key of BaseUser
        public required BaseUser User { get; set; } // Navigation property
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string? Name { get; set; }
    }

    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public string? UserId { get; set; } // Foreign key with the same type as the primary key of BaseUser
        public BaseUser User { get; set; } // Navigation property
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        public DateTime DateTime { get; set; }
        public string? Description { get; set; }
    }
}
