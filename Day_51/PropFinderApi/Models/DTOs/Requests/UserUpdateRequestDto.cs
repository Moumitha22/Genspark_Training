using PropFinderApi.Attributes;

namespace PropFinderApi.Models.DTOs
{
    public class UserUpdateRequestDto
    {
        [OptionalName]
        public string? Name { get; set; }

        [OptionalPhoneNumber]
        public string? PhoneNumber { get; set; }
    }

}