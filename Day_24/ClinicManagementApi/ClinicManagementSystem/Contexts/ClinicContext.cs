using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Contexts
{
    public class ClinicContext : DbContext
    {
        public ClinicContext(DbContextOptions options) : base(options)
        {

        }
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseNpgsql("User ID=postgres;Password=P@ssw0rd;Host=localhost;Port=5433;Database=myDataBase;");
        // }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<DoctorSpeciality> DoctorSpecialities { get; set; }
        public DbSet<FileFormat> FileFormats { get; set; }


        public DbSet<User> Users { get; set; }
        public DbSet<DoctorsBySpecialityResponseDto> DoctorsBySpeciality { get; set; }

        public async Task<List<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
        {
            return await this.Set<DoctorsBySpecialityResponseDto>()
                        .FromSqlInterpolated($"select * from proc_GetDoctorsBySpeciality({speciality})")
                        .ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.Email)
                .HasConstraintName("FK_User_Patient")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.Email)
                .HasConstraintName("FK_User_Doctor")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasKey("AppointmentNumber")
                .HasName("PK_AppointmentNumber");

            modelBuilder.Entity<Appointment>()
                .HasOne(app => app.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(app => app.PatientId)
                .HasConstraintName("FK_Appointment_Patient")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(app => app.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(app => app.DoctorId)
                .HasConstraintName("FK_Appointment_Doctor")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>()
                .HasKey(ds => ds.SerialNumber);

            modelBuilder.Entity<DoctorSpeciality>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSpecialities)
                .HasForeignKey(ds => ds.DoctorId)
                .HasConstraintName("FK_DoctorSpeciality_Doctor")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>()
                .HasOne(ds => ds.Speciality)
                .WithMany(s => s.DoctorSpecialities)
                .HasForeignKey(ds => ds.SpecialityId)
                .HasConstraintName("FK_DoctorSpeciality_Speciality")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}