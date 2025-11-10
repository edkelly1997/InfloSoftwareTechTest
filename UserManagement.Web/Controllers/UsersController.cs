using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using System.Text.Json;

namespace UserManagement.WebMS.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserAuditLogService _userAuditLogService;
    public UsersController(IUserService userService, IUserAuditLogService userAuditLogService)
    {
        _userService = userService;
        _userAuditLogService = userAuditLogService;
    }

    [HttpGet]
    public async Task<ViewResult> List(bool allUsers = true, bool isActive = true)
    {
        IEnumerable<UserListItemViewModel> items;

        if (allUsers)
        {
            var users = await _userService.GetAll();
            items = users.Select(users => new UserListItemViewModel
            {
                Id = users.Id,
                Forename = users.Forename,
                Surname = users.Surname,
                Email = users.Email,
                IsActive = users.IsActive,
                DateOfBirth = users.DateOfBirth
            });
        }
        else
        {
            var users = await _userService.FilterByActive(isActive);
            items = users.Select(users => new UserListItemViewModel
            {
                Id = users.Id,
                Forename = users.Forename,
                Surname = users.Surname,
                Email = users.Email,
                IsActive = users.IsActive,
                DateOfBirth = users.DateOfBirth
            });
        }

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AddEditUser(long id)
    {
        UserDetailsViewModel user = new UserDetailsViewModel();

        if (id != 0)
        {
            var existingUser = await _userService.GetById(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            user = new UserDetailsViewModel
            {
                Id = existingUser.Id,
                Forename = existingUser.Forename,
                Surname = existingUser.Surname,
                Email = existingUser.Email,
                IsActive = existingUser.IsActive,
                DateOfBirth = existingUser.DateOfBirth
            };

            user.isActiveOptions = new SelectList(new[]
            {
                new { Value = "true", Text = "Active" },
                new { Value = "false", Text = "Inactive" }
            }, "Value", "Text", existingUser.IsActive.ToString().ToLower());
        }
        else
        {
            user.IsActive = true;
            user.isActiveOptions = new SelectList(new[]
            {
                new { Value = "true", Text = "Active" },
                new { Value = "false", Text = "Inactive" }
            }, "Value", "Text", user.IsActive.ToString().ToLower());
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> AddEditUser(UserDetailsViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = new User
            {
                Id = model.Id,
                Forename = model.Forename!,
                Surname = model.Surname!,
                Email = model.Email!,
                IsActive = model.IsActive,
                DateOfBirth = model.DateOfBirth
            };

            var auditLog = new UserAuditLog();

            if (model.Id == 0)
            {
                await _userService.Create(user);
                auditLog = new UserAuditLog
                {
                    UserId = user.Id,
                    ChangeType = "User Created",
                    UserFullName = $"{user.Forename} {user.Surname}",
                    ChangedAt = DateTime.UtcNow,
                    ChangedData = JsonSerializer.Serialize(new
                    {
                        OldDetails = "N/A",
                        NewDetails = user
                    })
                };
            }
            else
            {
                var oldUserDetails = await _userService.GetById(model.Id);

                if (oldUserDetails == null)
                {
                    return NotFound();
                }

                var oldDetailsForAudit = new User
                {
                    Id = oldUserDetails.Id,
                    Forename = oldUserDetails.Forename,
                    Surname = oldUserDetails.Surname,
                    Email = oldUserDetails.Email,
                    IsActive = oldUserDetails.IsActive,
                    DateOfBirth = oldUserDetails.DateOfBirth
                };

                oldUserDetails.Forename = model.Forename!;
                oldUserDetails.Surname = model.Surname!;
                oldUserDetails.Email = model.Email!;
                oldUserDetails.IsActive = model.IsActive;
                oldUserDetails.DateOfBirth = model.DateOfBirth;

                await _userService.Update(oldUserDetails);
                auditLog = new UserAuditLog
                {
                    UserId = user.Id,
                    ChangeType = "User Updated",
                    UserFullName = $"{user.Forename} {user.Surname}",
                    ChangedAt = DateTime.UtcNow,
                    ChangedData = JsonSerializer.Serialize(new
                    {
                        OldDetails = oldDetailsForAudit,
                        NewDetails = user
                    })
                };
            }

            await _userAuditLogService.Create(auditLog);

            return RedirectToAction("List");
        }
        else
        {
            model.isActiveOptions = new SelectList(new[]
            {
            new { Value = "true", Text = "Active" },
            new { Value = "false", Text = "Inactive" }
        }, "Value", "Text", model.IsActive.ToString().ToLower());
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ViewDeleteUser (long id, bool isDelete)
    {
        var existingUser = await _userService.GetById(id);
        if (existingUser == null)
        {
            return NotFound();
        }
        var isActive = "Active";
        if (!existingUser.IsActive)
        {
            isActive = "Inactive";
        }
        var viewModel = new UserDetailsReadOnlyViewModel
        {
            Id = existingUser.Id,
            Forename = existingUser.Forename,
            Surname = existingUser.Surname,
            Email = existingUser.Email,
            IsActive = isActive,
            DateOfBirth = existingUser.DateOfBirth,
            IsDelete = isDelete
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(UserDetailsReadOnlyViewModel model)
    {
        var existingUser = await _userService.GetById(model.Id);
        if (existingUser == null)
        {
            return NotFound();
        }

        await _userService.Delete(existingUser);
        return RedirectToAction("List");
    }
}
