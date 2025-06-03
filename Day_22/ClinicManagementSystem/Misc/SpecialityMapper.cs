using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Misc
{
    public class SpecialityMapper
    {
        public Speciality MapSpecialityAddRequestDtoToSpeciality(SpecialityAddRequestDto addRequestDto)
        {
            Speciality speciality = new();
            speciality.Name = addRequestDto.Name;
            speciality.Status = "Active";

            return speciality;
        }
        public DoctorSpeciality MapDoctorSpeciality(int doctorId, int specialityId)
        {
            DoctorSpeciality doctorSpeciality = new();
            doctorSpeciality.DoctorId = doctorId;
            doctorSpeciality.SpecialityId = specialityId;

            return doctorSpeciality;
        }
    }

}