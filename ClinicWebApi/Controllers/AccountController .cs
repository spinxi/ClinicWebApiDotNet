using ClinicWebApi.Data;
using ClinicWebApi.DTO;
using ClinicWebApi.Models;
using ClinicWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace ClinicWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<BaseUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext, UserManager<BaseUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new BaseUser
            {
                
                Email = model.Email,
                IdNumber =model.IdNumber,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                
                CreatedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);


                /*                var confirmationLink = Url.Action("confirmEmail",
                                    new { userId = user.Id, token }, Request.Scheme);*/

                var frontEndUrl = _configuration["FrontendUrl"];
                var confirmationLink = $"{frontEndUrl}/confirm-email?userId={user.Id}&code={token}";

                // Compose email message
                var subject = $"ელ.ფოსტის დასტური";
                var htmlMessage = $"გთხოვთ დაადასტუროთ თქვენი ელ.ფოსტა <a href='{confirmationLink}'>აქ დაკლიკებით</a>.";

                // Send confirmation email
                await _emailSender.SendEmailAsync(user.Email, subject, htmlMessage);

                // Automatically sign in the user after registration
                // ვფიქრობ რამდენად გამოსადეგია ეს ვარიანტი მომხმარებელმა ავტორიზაცია რო გაიაროს
                // ავტომატურად რეგისტრაციის შემდეგ

                // await _signInManager.SignInAsync(user, isPersistent: false);

                // User registration successful
                return Ok(new { message = "Registration successful. Confirmation email sent." });
            }
            else
            {
                // User registration failed
                return BadRequest(new { errors = result.Errors });
            }
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest(new { message = "Invalid parameters for email confirmation." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });

            }

            // Check if the email has already been confirmed
            if (user.EmailConfirmed)
            {
                return Ok(new { message = "Email address already confirmed." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to confirm email address." });
            }

            // Update the user's email confirmation status
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            
            return Ok("Email address confirmed successfully.");
        }


        [HttpPost("resendConfirmationEmail")]
        public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailDTO model)
        {
            string email = model.Email;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email address is required." });
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.Email == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.EmailConfirmed)
            {
                return Ok(new { message = "Email address already confirmed." });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var frontEndUrl = _configuration["FrontendUrl"];
            var confirmationLink = $"{frontEndUrl}/confirm-email?userId={user.Id}&code={token}";

            var subject = $"ელ.ფოსტის დასტური";
            var htmlMessage = $"გთხოვთ დაადასტუროთ თქვენი ელ.ფოსტა <a href='{confirmationLink}'>აქ დაკლიკებით</a>.";

            await _emailSender.SendEmailAsync(user.Email, subject, htmlMessage);

            return Ok(new { message = "Confirmation email resent successfully." });
        }



        [Authorize]
        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            // Get the current user's ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User ID not found in claims." });
            }

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            try
            {
                // Get the bookings for the current user
                var bookings = await _dbContext.Bookings
                    .Where(b => b.UserId == userId)
                    .OrderBy(b => b.DateTime)
                    .ToListAsync();

                // Extract the booking count
                var bookingCount = bookings?.Count;

                // Extract the booking dates
                var bookingDates = bookings?.Select(b => b.DateTime).ToList();

                // Return the user information along with the booking count and dates
                return Ok(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    IdNumber = user.IdNumber,
                    BookingCount = bookingCount,
                    BookingDates = bookingDates
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while retrieving user information. {ex}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication(bool enableTwoFactorAuthentication)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { message = $"Unable to load user with ID '{_userManager.GetUserId(User)}'." });
            }

            if (enableTwoFactorAuthentication)
            {
                // Enable 2FA for the user
                user.TwoFactorEnabled = true;
            }
            else
            {
                // Disable 2FA for the user
                user.TwoFactorEnabled = false;
            }

            await _userManager.UpdateAsync(user);

            return Ok( new {message = "2FA succesfully enabled" });
        }
    }

}


