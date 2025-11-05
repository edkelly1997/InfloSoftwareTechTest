using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UserManagement.Web.Models.Users;

public class UserDetailsViewModel
{
    public long Id { get; set; }
    [Required]
    public string? Forename { get; set; }
    [Required]
    public string? Surname { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string? Email { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [CustomValidation(typeof(UserDetailsViewModel), nameof(ValidateDateOfBirth))]
    public DateOnly? DateOfBirth { get; set; }
    public SelectList? isActiveOptions { get; set;}

    public static ValidationResult? ValidateDateOfBirth(DateOnly? date, ValidationContext context)
    {
        if (!date.HasValue)
        {
            return new ValidationResult("Please enter a valid date of birth.");
        }

        var dateOfBirth = date.Value;

        if (dateOfBirth.ToString("dd/MM/yyyy") == "01/01/0001")
        {
            return new ValidationResult("Please enter a valid date of birth.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (dateOfBirth > today)
        {
            return new ValidationResult("Date of birth cannot be in the future.");
        }

        if (dateOfBirth < today.AddYears(-130))
        {
            return new ValidationResult("Please enter a realistic date of birth.");
        }

        return ValidationResult.Success;
    }
}
