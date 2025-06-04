using System;
using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Misc;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;
using ClinicManagementSystem.Repositories;

namespace ClinicManagementSystem.Services;

public class AppointmentService : IAppointmentService
{
    AppointmentMapper appointmentMapper;
    private readonly IRepository<string, Appointment> _appointmentRepository;
    private readonly IDoctorService _doctorService;

    public AppointmentService(IRepository<string, Appointment> appointmentRepository,IDoctorService doctorService)
    {
        _appointmentRepository = appointmentRepository;
        _doctorService = doctorService;
        appointmentMapper = new AppointmentMapper();
    }

    public async Task<Appointment> AddAppointment(AppointmentAddRequestDto appointmentAddRequestDto)
    {
        var appointment = appointmentMapper.MapAppointmentAddRequestDtoToAppointment(appointmentAddRequestDto);
        return await _appointmentRepository.Add(appointment);
    }


    public async Task<bool> CancelAppointment(string appointmentNumber, string email)
    {
        var doctor = await _doctorService.GetDoctorByEmail(email);

        var appointment = await _appointmentRepository.Get(appointmentNumber);

        if (appointment.DoctorId != doctor.Id)
            throw new Exception("You are not authorized to cancel this appointment.");

        appointment.Status = "Cancelled";
        await _appointmentRepository.Update(appointmentNumber, appointment);
        return true;
    }


    public async Task<ICollection<Appointment>> GetAllAppointments()
    {
        var appointments = await _appointmentRepository.GetAll();
        return appointments.ToList();
    }

    public async Task<Appointment> GetAppointmentById(string id)
    {
        var appointment = await _appointmentRepository.Get(id);
        return appointment;
    }

}
