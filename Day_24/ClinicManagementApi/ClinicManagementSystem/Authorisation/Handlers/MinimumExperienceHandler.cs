using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using ClinicManagementSystem.Authorisation.Requirements;
using ClinicManagementSystem.Repositories;
using ClinicManagementSystem.Interfaces;

namespace ClinicManagementSystem.Authorisation.Handlers
{
    public class MinimumExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
    {   
        private readonly IDoctorService _doctorService;

        public MinimumExperienceHandler(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumExperienceRequirement requirement)
        {
            // Extract the user's email from claims
            var usernameClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            var role = context.User.FindFirst(ClaimTypes.Role);

            if (role == null || role.Value != "Doctor" || usernameClaim == null)
            {
                return;
            }

            var email = usernameClaim.Value;

            var doctor = await _doctorService.GetDoctorByEmail(email);
            if (doctor == null)
            {
                return;
            }

            if (doctor.YearsOfExperience >= requirement.MinimumExperienceYears)
            {
                context.Succeed(requirement);
            }
        }
    }
}
