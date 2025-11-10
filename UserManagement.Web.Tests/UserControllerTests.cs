using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public async Task List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();
        _userService.Setup(_userService => _userService.GetAll()).ReturnsAsync(users);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        var model = (result as ViewResult)!.Model;
        model.Should().BeOfType<UserListViewModel>();
        ((UserListViewModel)model).Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task List_ActiveUsers_ModelMustContainOnlyActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(allUsers: false, isActive: true);
        _userService.Setup(_userService => _userService.FilterByActive(true)).ReturnsAsync(users);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List(allUsers: false, isActive: true);

        var model = (result as ViewResult)!.Model;
        model.Should().BeOfType<UserListViewModel>();
        ((UserListViewModel)model).Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task List_InactiveUsers_ModelMustContainOnlyInactiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(allUsers: false, isActive: false);
        _userService.Setup(_userService => _userService.FilterByActive(false)).ReturnsAsync(users);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List(allUsers: false, isActive: false);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var model = (result as ViewResult)!.Model;
        model.Should().BeOfType<UserListViewModel>();
        ((UserListViewModel)model).Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task AddEditUser_Get_WithValidId_ReturnsUserView()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().First();
        _userService.Setup(s => s.GetById(user.Id)).ReturnsAsync(user);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddEditUser(user.Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model;
        model.Should().BeOfType<UserDetailsViewModel>();
        ((UserDetailsViewModel)model).Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task AddEditUser_Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        _userService.Setup(s => s.GetById(999)).ReturnsAsync((User?)null);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddEditUser(999);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddEditUser_Post_WithNewUser_CallsCreateAndRedirects()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().First();
        var model = new UserDetailsViewModel
        {
            Id = 0,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddEditUser(model);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(s => s.Create(It.Is<User>(u => u.Forename == user.Forename)), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>();
    }

    [Fact]
    public async Task AddEditUser_Post_WithExistingUser_CallsUpdateAndRedirects()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().First();
        var model = new UserDetailsViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };
        _userService.Setup(s => s.GetById(user.Id)).ReturnsAsync(user);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.AddEditUser(model);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(s => s.Update(It.Is<User>(u => u.Id == user.Id)), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>();
    }

    [Fact]
    public async Task ViewDeleteUser_Get_WithValidId_ReturnsViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().Last();
        _userService.Setup(s => s.GetById(user.Id)).ReturnsAsync(user);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.ViewDeleteUser(user.Id, true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model;
        model.Should().BeOfType<UserDetailsReadOnlyViewModel>();
        ((UserDetailsReadOnlyViewModel)model).Id.Should().Be(user.Id);
        ((UserDetailsReadOnlyViewModel)model).IsDelete.Should().BeTrue();
    }

    [Fact]
    public async Task ViewDeleteUser_Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        _userService.Setup(s => s.GetById(999)).ReturnsAsync((User?)null);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.ViewDeleteUser(999, true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteUser_Post_CallsDeleteAndRedirects()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().Last();
        _userService.Setup(s => s.GetById(user.Id)).ReturnsAsync(user);
        var model = new UserDetailsReadOnlyViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive ? "Active" : "Inactive",
            DateOfBirth = user.DateOfBirth,
            IsDelete = true
        };

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.DeleteUser(model);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(s => s.Delete(It.Is<User>(u => u.Id == user.Id)), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>();
    }

    private User[] SetupUsers(bool allUsers = true, bool isActive = true)
    {

        var ActiveUser = new[]
        {
            new User
            {
                Id = 1,
                Forename = "Johnny",
                Surname = "User",
                Email = "juser@example.com",
                IsActive = true,
                DateOfBirth = DateOnly.Parse("01/01/2001")
            }
        };

        var InActiveUser = new[]
        {
            new User
            {
                Id = 2,
                Forename = "Fred",
                Surname = "IsInactive",
                Email = "fisinactive@test.com",
                IsActive = false,
                DateOfBirth = DateOnly.Parse("01/01/2001")
            }
        };

        User[] users = allUsers ? ActiveUser.Concat(InActiveUser).ToArray() : isActive ? ActiveUser : InActiveUser;

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IUserAuditLogService> _userAuditLogService = new();
    private UsersController CreateController() => new(_userService.Object, _userAuditLogService.Object);
}
