using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionAPI.Validation;

public class NameValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string name = value as string ?? "";
        if (string.IsNullOrEmpty(name))
            return false;
        foreach (char c in name)
        {
            if (char.IsWhiteSpace(c) || !char.IsLetterOrDigit(c))
                return false;
        }
        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} is a Invalid name, please ensure your should not contain any special chararacter or empty white space";
    }
}
