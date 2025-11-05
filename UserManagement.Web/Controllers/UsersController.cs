using System.Linq;
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
        UserListItemViewModel user = new UserListItemViewModel();

        if (id != 0)
        {
            var existingUser = _userService.GetById(id).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound();
            }
            user = new UserListItemViewModel
            {
                Id = existingUser.Id,
                Forename = existingUser.Forename,
                Surname = existingUser.Surname,
                Email = existingUser.Email,
                IsActive = existingUser.IsActive,
                DateOfBirth = existingUser.DateOfBirth
            };
        }

        return View(user);
    }
}
