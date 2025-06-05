using System.ComponentModel.DataAnnotations;
namespace ClinicManagementSystem.Misc
{
    public class NameValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            string name = (value as string ?? "").Trim();
            if(string.IsNullOrEmpty(name))
              return false;
            foreach(char c in name)
            {
                if(!char.IsLetter(c) && !char.IsWhiteSpace(c))
        return false;
            }
            return true;
        }
    }
}