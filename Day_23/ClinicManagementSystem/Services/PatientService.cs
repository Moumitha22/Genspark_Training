using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;
using ClinicManagementSystem.Misc;
using AutoMapper;

namespace ClinicManagementSystem.Services
{
    public class PatientService : IPatientService
    {
        PatientMapper patientMapper;
        private readonly IRepository<int, Patient> _patientRepository;
        private readonly IRepository<string, User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;

        public PatientService(IRepository<int, Patient> patientRepository,
                            IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IMapper mapper)
        {
            patientMapper = new PatientMapper();
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _mapper = mapper;

        }

        public async Task<Patient> AddPatient(PatientAddRequestDto PatientDto)
        {
            try
            {
                var user = _mapper.Map<PatientAddRequestDto, User>(PatientDto);

                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = PatientDto.Password
                });

                user.Password = encryptedData.EncryptedData;
                user.HashKey = encryptedData.HashKey;
                user.Role = "Patient";

                user = await _userRepository.Add(user);

                var newPatient = patientMapper.MapPatientAddRequestDtoToPatient(PatientDto);
                newPatient = await _patientRepository.Add(newPatient);

                if (newPatient == null)
                    throw new Exception("Could not add Patient");

                return newPatient;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<ICollection<Patient>> GetAllPatients()
        {
            var patients = await _patientRepository.GetAll();
            return patients.ToList();
        }

        public async Task<Patient> GetPatientById(int id)
        {
            var patient = await _patientRepository.Get(id);

            return patient;
        }
    }
}