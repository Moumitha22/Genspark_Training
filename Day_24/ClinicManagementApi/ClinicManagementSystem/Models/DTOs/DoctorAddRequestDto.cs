using System; 
using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Misc;

namespace ClinicManagementSystem.Models.DTOs
{
    public class DoctorAddRequestDto
    {
        [NameValidation(ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; } = string.Empty;
        public float YearsOfExperience { get; set; }
        public ICollection<SpecialityAddRequestDto>? Specialities { get; set; }

        public string Email { get; set; }= string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}