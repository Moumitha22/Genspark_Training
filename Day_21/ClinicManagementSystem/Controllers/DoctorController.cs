
using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<DoctorResponseDto>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctors();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctorById(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorById(id);
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> AddDoctor([FromBody] DoctorAddRequestDto doctorDto)
        {
            try
            {
                var createdDoctor = await _doctorService.AddDoctor(doctorDto);
                return Ok(createdDoctor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctorByName(string name)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByName(name);
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("by-speciality/{speciality}")]
        public async Task<ActionResult<ICollection<DoctorsBySpecialityResponseDto>>> GetDoctorsBySpeciality(string speciality)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsBySpeciality(speciality);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}