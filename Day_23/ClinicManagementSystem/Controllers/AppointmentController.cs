using System.Security.Claims;
using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> AddAppointment([FromBody] AppointmentAddRequestDto dto)
        {
            try
            {
                var appointment = await _appointmentService.AddAppointment(dto);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding appointment: {ex.Message}");
            }
        }

        [HttpPut("cancel/{appointmentNumber}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Doctor", Policy = "MinimumExperience3")]
        public async Task<ActionResult> CancelAppointment(string appointmentNumber)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized("Invalid token");

                var result = await _appointmentService.CancelAppointment(appointmentNumber, userEmail);
                return Ok($"Appointment {appointmentNumber} cancelled successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Appointment>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointments();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving appointments: {ex.Message}");
            }
        }

        [HttpGet("{appointmentNumber}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(string appointmentNumber)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentById(appointmentNumber);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return NotFound($"Appointment not found: {ex.Message}");
            }
        }
    }
}
