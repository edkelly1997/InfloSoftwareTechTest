using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public ViewResult List(bool allUsers = true, bool isActive = true)
    {
        IEnumerable<UserListItemViewModel> items;

        if (allUsers)
        {
            items = _userService.GetAll().Select(p => new UserListItemViewModel
            {
                Id = p.Id,
                Forename = p.Forename,
                Surname = p.Surname,
                Email = p.Email,
                IsActive = p.IsActive,
                DateOfBirth = p.DateOfBirth
            });
        }
        else
        {
            items = _userService.FilterByActive(isActive).Select(p => new UserListItemViewModel
            {
                Id = p.Id,
                Forename = p.Forename,
                Surname = p.Surname,
                Email = p.Email,
                IsActive = p.IsActive,
                DateOfBirth = p.DateOfBirth
            });
        }

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult AddEditUser(long id)
    {
        UserDetailsViewModel user = new UserDetailsViewModel();

        if (id != 0)
        {
            var existingUser = _userService.GetById(id).FirstOrDefault();
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
    public IActionResult AddEditUser(UserDetailsViewModel model)
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
            if (model.Id == 0)
            {
                _userService.Create(user);
            }
            else
            {
                _userService.Update(user);
            }

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
    public IActionResult ViewDeleteUser (long id, bool isDelete)
    {
        var existingUser = _userService.GetById(id).FirstOrDefault();
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
    public IActionResult DeleteUser(UserDetailsReadOnlyViewModel model)
    {
        var existingUser = _userService.GetById(model.Id).FirstOrDefault();
        if (existingUser == null)
        {
            return NotFound();
        }

        _userService.Delete(existingUser);
        return RedirectToAction("List");
    }
}
