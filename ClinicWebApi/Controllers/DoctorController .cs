using Microsoft.AspNetCore.Mvc;
using ClinicWebApi.Models;
using System;
using System.Linq;
using ClinicWebApi.Data;
using Microsoft.AspNetCore.Authorization;
using ClinicWebApi.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ClinicWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Doctor
        [HttpGet]
        public IActionResult GetDoctors()
        {
            var doctors = _context.Doctors
                .Include(d => d.Category)
                .Include(d => d.User)
                .OrderByDescending(d => d.User.Pinned)
                .Select(d => new {
                    d.DoctorId,
                    d.Category,
                    FirstName = d.User.FirstName,
                    LastName = d.User.LastName,
                    Pinned = d.User.Pinned,
                })
                .ToList();

            return Ok(doctors);
        }

        // GET: api/Doctor/5
        [HttpGet("{id}")]
        public IActionResult GetDoctor(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        // POST: api/Doctor
        [Authorize]
        [HttpPost]
        public IActionResult AddDoctor(UserRegistrationDTO userRegistration, int categoryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Hash the password securely
            var passwordHasher = new PasswordHasher<BaseUser>();
            string passwordHash = passwordHasher.HashPassword(null, "Doctorpass1!");


            // Normalize the email
            string normalizedEmail = userRegistration.Email.ToUpper();
            // Create a new BaseUser object using the data from UserRegistrationDTO
            var baseUser = new BaseUser
            {
                Email = userRegistration.Email,
                IdNumber = userRegistration.IdNumber,
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                UserName = userRegistration.Email,
                PasswordHash = passwordHash,
                NormalizedEmail = normalizedEmail,
            };

            // Add the new BaseUser to the context
            _context.Users.Add(baseUser);
            _context.SaveChanges();

            // Create a new Doctor object and set its properties
            var doctor = new Doctor
            {
                User = baseUser,
                CategoryId = categoryId // Set CategoryId from the parameter
            };

            // Add the new Doctor to the context
            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            // You may return some response here, such as the created DoctorId
            return Ok(new { doctor.DoctorId });
        }


        // PUT: api/Doctor/5
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateDoctor(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return BadRequest();
            }

            _context.Entry(doctor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Doctor/5
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
