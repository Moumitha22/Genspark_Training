using System;

namespace ClinicManagementSystem.Models.DTOs
{
    public class DoctorsBySpecialityResponseDto
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public float Yoe { get; set; }
    }
}