using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;
using ClinicManagementSystem.Misc;
using AutoMapper;

namespace ClinicManagementSystem.Services
{
    public class DoctorService : IDoctorService
    {
        DoctorMapper doctorMapper ;
        SpecialityMapper specialityMapper;
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IOtherContextFunctionities _otherContextFunctionities;
        private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;

        public DoctorService(IRepository<int, Doctor> doctorRepository,
                            IRepository<int, Speciality> specialityRepository,
                            IRepository<int, DoctorSpeciality> doctorSpecialityRepository,
                            IRepository<string,User> userRepository,
                            IOtherContextFunctionities otherContextFunctionities,
                            IEncryptionService encryptionService,
                            IMapper mapper)
        {
            doctorMapper = new DoctorMapper();
            specialityMapper = new();
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
            _userRepository = userRepository;
            _otherContextFunctionities = otherContextFunctionities;
            _encryptionService = encryptionService;
            _mapper = mapper;

        }
        
        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctorDto)
        {
            try
            {
                var user = _mapper.Map<DoctorAddRequestDto, User>(doctorDto);
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = doctorDto.Password 
                });
                user.Password = encryptedData.EncryptedData;
                user.HashKey = encryptedData.HashKey;
                user.Role = "Doctor";

                user = await _userRepository.Add(user);

                var newDoctor = doctorMapper.MapDoctorAddRequestDtoToDoctor(doctorDto);
                newDoctor = await _doctorRepository.Add(newDoctor);

                if (newDoctor == null)
                    throw new Exception("Could not add doctor");

                if (doctorDto.Specialities.Count() > 0)
                {
                    int[] specialityIds = await MapAndAddSpecialities(doctorDto.Specialities);
                    foreach (var specialityId in specialityIds)
                    {
                        var newDoctorSpeciality = specialityMapper.MapDoctorSpeciality(newDoctor.Id, specialityId);
                        newDoctorSpeciality = await _doctorSpecialityRepository.Add(newDoctorSpeciality);
                    }
                }
                return newDoctor;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        private async Task<int[]> MapAndAddSpecialities(ICollection<SpecialityAddRequestDto> specialityAddRequestDtos) {
            int[] specialityIds = new int[specialityAddRequestDtos.Count()];

            IEnumerable<Speciality> existingSpecialities = null;
            try
            {
                existingSpecialities = await _specialityRepository.GetAll();
            }
            catch (Exception e)
            {

            }

            int count = 0;
            foreach (var specialityAddRequestDto in specialityAddRequestDtos)
            {
                Speciality speciality = null;
                if (existingSpecialities != null)
                {
                    speciality = existingSpecialities.FirstOrDefault(s => s.Name.ToLower() == specialityAddRequestDto.Name.ToLower());

                }
                if (speciality == null) // null if 1) No existingSpecialities 2) No matching speciality in existingSpecialities 
                {
                    speciality = specialityMapper.MapSpecialityAddRequestDtoToSpeciality(specialityAddRequestDto);
                    speciality = await _specialityRepository.Add(speciality);
                }
                specialityIds[count++] = speciality.Id;
            }
            return specialityIds;
        }

        public async Task<ICollection<DoctorResponseDto>> GetAllDoctors()
        {
            var doctors = await _doctorRepository.GetAll();
            return doctors.Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
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
                Email = doctor.Email,
                Status = doctor.Status,
                Specialities = doctor.DoctorSpecialities?
                                .Select(ds => ds.Speciality?.Name ?? "")
                                .ToList()
            };

            return doctorResponseDto;
        }

        public async Task<Doctor> GetDoctorByEmail(string email)
        {
            var doctors = await _doctorRepository.GetAll();
            var doctor = doctors.FirstOrDefault(d => d.Email != null && d.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
            {
                throw new Exception($"Doctor with email '{email}' not found.");
            }
            return doctor;
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
                Email = doctor.Email,
                Specialities = doctor.DoctorSpecialities?
                                .Select(ds => ds.Speciality?.Name ?? "")
                                .ToList()
            };
        }
        public async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
        {
            var result = await _otherContextFunctionities.GetDoctorsBySpeciality(speciality);
            return result;
        }
        
    }
}
