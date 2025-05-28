
namespace ClinicManagementSystem.Models.DTOs
{
    public class DoctorResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public float YearsOfExperience { get; set; }
        public string Status { get; set; } = "";
        public List<string>? Specialities { get; set; }
    }
}
