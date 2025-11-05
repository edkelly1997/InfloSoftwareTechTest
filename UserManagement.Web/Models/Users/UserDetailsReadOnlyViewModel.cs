using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UserManagement.Web.Models.Users;

public class UserDetailsReadOnlyViewModel
{
    public long Id { get; set; }
    public string? Forename { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? IsActive { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public SelectList? isActiveOptions { get; set;}
    public bool IsDelete { get; set; }
}
