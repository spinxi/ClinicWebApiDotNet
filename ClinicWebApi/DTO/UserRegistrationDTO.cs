namespace ClinicWebApi.DTO
{
    public class UserRegistrationDTO
    {
        /*public required string UserName { get; set; }*/
        public required string Email { get; set; }
        public required string IdNumber { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
