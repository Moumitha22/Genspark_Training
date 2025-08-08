using System.ComponentModel.DataAnnotations;
using OnlineAuctionAPI.Models;
namespace OnlineAuctionAPI.Validation;

public class RoleValidation : ValidationAttribute
{
    private readonly string[] _validRoles =
    {
        UserRole.Seller.ToString(),
        UserRole.Bidder.ToString(),
        UserRole.Admin.ToString()
    };

    public override bool IsValid(object? value)
    {
        if (value is not string role || string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        return _validRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The role '{name}' is not valid. Please provide a valid role like {string.Join(", ", _validRoles)}.";
    }
    
}