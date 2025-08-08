namespace OnlineAuctionAPI.Validation;

using System.ComponentModel.DataAnnotations;
using OnlineAuctionAPI.Helpers;

public class PasswordValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string password = value as string ?? "";
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(
                $"{nameof(password)} must not be null, empty or whitespace"
            );
        if (!(password.Length > 3) || password.Contains(' '))
            return false;
        string allowedSpecials = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/`~\\";
        foreach (char c in password)
        {
            if (!char.IsLetterOrDigit(c) && !allowedSpecials.Contains(c))
                return false;
        }
        return true;
    }
}
