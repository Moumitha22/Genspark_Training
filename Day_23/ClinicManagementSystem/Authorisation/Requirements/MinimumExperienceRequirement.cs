using Microsoft.AspNetCore.Authorization;

namespace ClinicManagementSystem.Authorisation.Requirements
{
    public class MinimumExperienceRequirement : IAuthorizationRequirement
    {
        public int MinimumExperienceYears { get; }

        public MinimumExperienceRequirement(int years)
        {
            MinimumExperienceYears = years;
        }
    }
}
