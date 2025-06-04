
using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> AddPatient([FromBody] PatientAddRequestDto PatientDto)
        {
            try
            {
                var createdPatient = await _patientService.AddPatient(PatientDto);
                return Ok(createdPatient);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<ICollection<Patient>>> GetAllPatients()
        {
            try
            {
                var patients = await _patientService.GetAllPatients();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(int id)
        {
            try
            {
                var patient = await _patientService.GetPatientById(id);
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}