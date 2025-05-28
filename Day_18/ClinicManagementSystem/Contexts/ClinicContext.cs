using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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