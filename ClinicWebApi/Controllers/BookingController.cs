using ClinicWebApi.Data;
using ClinicWebApi.DTO;
using ClinicWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public BookingController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto bookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Ensure that the provided UserId and DoctorId exist
                var existingUser = await _dbContext.Users.FindAsync(bookingDto.UserId);
                var existingDoctor = await _dbContext.Doctors.FindAsync(bookingDto.DoctorId);

                if (existingUser == null)
                {
                    return BadRequest("User with the provided ID does not exist.");
                }

                if (existingDoctor == null)
                {
                    return BadRequest("Doctor with the provided ID does not exist.");
                }

                var booking = new Booking
                {
                    UserId = bookingDto.UserId,
                    User = existingUser,
                    DoctorId = bookingDto.DoctorId,
                    Doctor = existingDoctor,
                    DateTime = bookingDto.DateTime,
                    Description = bookingDto.Description
                };

                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync();

                var bookingDtoResponse = new CreateBookingDto
                {
                    UserId = booking.UserId,
                    DoctorId = booking.DoctorId,
                    DateTime = booking.DateTime,
                    Description = booking.Description
                };

                // Return the DTO
                return Ok(new { message = bookingDtoResponse });
            }
            catch (Exception ex)
            {
                // Handle potential data access errors
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetBookingsByDoctor(int doctorId)
        {
            try
            {
                // Retrieve doctor information
                var doctor = await _dbContext.Doctors.FindAsync(doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found." });
                }

                // Retrieve bookings for the doctor
                var bookings = await _dbContext.Bookings
                    .Include(b => b.User) // Include user information for each booking
                    .Where(b => b.DoctorId == doctorId)
                    .ToListAsync();

                if (bookings == null || bookings.Count == 0)
                {
                    return NotFound(new { message = "No bookings found for this doctor." });
                }

                // Return bookings along with doctor information
                return Ok(new
                {
                    Doctor = new
                    {
                        doctor.DoctorId,
                        doctor.User?.Email,
                        doctor.User?.FirstName,
                        doctor.User?.LastName,
                    },
                    Bookings = bookings.Select(b => new
                    {
                        b.BookingId,
                        b.UserId,
                        User = new
                        {
                            b.User.FirstName,
                            b.User.LastName,
                            b.User.Email,
                        },
                        b.DateTime,
                        b.Description
                    })
                });
            }
            catch (Exception ex)
            {
                // Handle potential errors
                return StatusCode(500, new { message = "An error occurred while retrieving bookings." });
            }
        }
    }
}
