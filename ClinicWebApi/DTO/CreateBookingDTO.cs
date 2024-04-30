namespace ClinicWebApi.DTO
{
    public class CreateBookingDto
    {
        public string UserId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
    }
}
