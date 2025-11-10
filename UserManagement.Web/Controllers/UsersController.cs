using System.Linq;
using System.Threading.Tasks;
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
            if (model.Id == 0)
            {
                await _userService.Create(user);
            }
            else
            {
                await _userService.Update(user);
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
