using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;

        public DoctorService(IRepository<int, Doctor> doctorRepository,
                             IRepository<int, Speciality> specialityRepository,
                             IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
        }

        public async Task<DoctorResponseDto> AddDoctor(DoctorAddRequestDto doctorDto)
        {
            var doctor = new Doctor
            {
                Name = doctorDto.Name,
                YearsOfExperience = doctorDto.YearsOfExperience,
                Status = "Active"
            };

            var createdDoctor = await _doctorRepository.Add(doctor);

            if (doctorDto.Specialities != null)
            {
                var uniqueSpecialityNames = doctorDto.Specialities
                    .Select(s => s.Name.Trim().ToLower())
                    .Distinct();

                var existingSpecialities = await _specialityRepository.GetAll();

                foreach (var specialityName in uniqueSpecialityNames)

                {
                    var existingSpeciality = existingSpecialities.FirstOrDefault(s => s.Name.Equals(specialityName, StringComparison.OrdinalIgnoreCase));

                    Speciality speciality;

                    if (existingSpeciality != null)
                    {
                        speciality = existingSpeciality;
                    }
                    else
                    {
                        speciality = new Speciality
                        {
                            Name = specialityName,
                            Status = "Active"
                        };
                        speciality = await _specialityRepository.Add(speciality);
                    }

                    var doctorSpeciality = new DoctorSpeciality
                    {
                        DoctorId = createdDoctor.Id,
                        SpecialityId = speciality.Id,
                    };
                    await _doctorSpecialityRepository.Add(doctorSpeciality);
                }
            }

            var fullDoctor = await _doctorRepository.Get(createdDoctor.Id);

            return new DoctorResponseDto
            {
                Id = fullDoctor.Id,
                Name = fullDoctor.Name,
                YearsOfExperience = fullDoctor.YearsOfExperience,
                Status = fullDoctor.Status,
                Specialities = fullDoctor.DoctorSpecialities?
                                .Select(ds => ds.Speciality?.Name ?? "")
                                .ToList()
            };
        }
        public async Task<ICollection<DoctorResponseDto>> GetAllDoctors()
        {
            var doctors = await _doctorRepository.GetAll();
            return doctors.Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                YearsOfExperience = d.YearsOfExperience,
                Status = d.Status,
                Specialities = d.DoctorSpecialities?
                                .Select(ds => ds.Speciality?.Name ?? "")
                                .ToList()
            }).ToList();
        }

        public async Task<DoctorResponseDto> GetDoctorById(int id)
        {
            var doctor = await _doctorRepository.Get(id);
            var doctorResponseDto = new DoctorResponseDto
            {
                Id = doctor.Id,
                Name = doctor.Name,
                YearsOfExperience = doctor.YearsOfExperience,
                Status = doctor.Status,
                Specialities = doctor.DoctorSpecialities?
                                .Select(ds => ds.Speciality?.Name ?? "")
                                .ToList()
            };

            return doctorResponseDto;
        }

        public async Task<DoctorResponseDto> GetDoctorByName(string name)
        {
            var doctors = await _doctorRepository.GetAll();
            var doctor = doctors.FirstOrDefault(d => d.Name != null && d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
            {
                throw new Exception($"Doctor with name '{name}' not found.");
            }

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                Name = doctor.Name,
                YearsOfExperience = doctor.YearsOfExperience,
                Status = doctor.Status,
                Specialities = doctor.DoctorSpecialities?
                                .Select(ds => ds.Speciality?.Name ?? "")
                                .ToList()
            };
        }

        public async Task<ICollection<DoctorResponseDto>> GetDoctorsBySpeciality(string speciality)
        {
            var allSpecialities = await _specialityRepository.GetAll();
            var matchedSpeciality = allSpecialities.FirstOrDefault(s => s.Name != null && s.Name.Equals(speciality, StringComparison.OrdinalIgnoreCase));

            if (matchedSpeciality == null)
            {
                throw new Exception("No speciality found");
            }

            var allDoctorSpecialities = await _doctorSpecialityRepository.GetAll();
            var doctorIds = allDoctorSpecialities
                            .Where(ds => ds.SpecialityId == matchedSpeciality.Id)
                            .Select(ds => ds.DoctorId)
                            .Distinct();

            var allDoctors = await _doctorRepository.GetAll();
            var filteredDoctors = allDoctors.Where(d => doctorIds.Contains(d.Id)).ToList();

            if (filteredDoctors.Count() == 0)
            {
                throw new Exception($"No doctors found with specuality {speciality}");
            }
            return filteredDoctors.Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                YearsOfExperience = d.YearsOfExperience,
                Status = d.Status,
                Specialities = d.DoctorSpecialities?
                                .Select(ds => ds.Speciality?.Name ?? "")
                                .ToList()
            }).ToList();
        }

        // Approach: Query doctors directly via the repository using eager loaded data
        // public async Task<ICollection<DoctorResponseDto>> GetDoctorsBySpeciality(string speciality)
        // {
        //     var allDoctors = await _doctorRepository.GetAll();

        //     var filteredDoctors = allDoctors.Where(d =>
        //         d.DoctorSpecialities != null &&
        //         d.DoctorSpecialities.Any(ds => ds.Speciality != null &&
        //                                     ds.Speciality.Name.Equals(speciality, StringComparison.OrdinalIgnoreCase))
        //     ).ToList();

        //     var responseDtos = filteredDoctors.Select(d => new DoctorResponseDto
        //     {
        //         Id = d.Id,
        //         Name = d.Name,
        //         YearsOfExperience = d.YearsOfExperience,
        //         Status = d.Status,
        //         Specialities = d.DoctorSpecialities?.Select(ds => ds.Speciality?.Name ?? "").ToList()
        //     }).ToList();

        //     if (responseDtos.Count == 0)
        //         throw new Exception($"No doctors found with speciality '{speciality}'");

        //     return responseDtos;
        // }
   
        
    }
}
