using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_ActiveUsers_ModelMustContainOnlyActiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(allUsers: false, isActive: true);

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(allUsers: false, isActive: true);

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_InactiveUsers_ModelMustContainOnlyInactiveUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers(allUsers: false, isActive: false);

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(allUsers: false, isActive: false);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void AddEditUser_Get_WithValidId_ReturnsUserView()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().First();
        _userService.Setup(s => s.GetById(user.Id)).Returns(new[] { user });

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.AddEditUser(user.Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>();
        var model = (result as ViewResult)!.Model;
        model.Should().BeOfType<UserDetailsViewModel>();
        ((UserDetailsViewModel)model).Id.Should().Be(user.Id);
    }

    [Fact]
    public void AddEditUser_Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        _userService.Setup(s => s.GetById(999)).Returns(Enumerable.Empty<User>());

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.AddEditUser(999);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void AddEditUser_Post_WithNewUser_CallsCreateAndRedirects()
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
        var result = controller.AddEditUser(model);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(s => s.Create(It.Is<User>(u => u.Forename == user.Forename)), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>();
    }

    [Fact]
    public void AddEditUser_Post_WithExistingUser_CallsUpdateAndRedirects()
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

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.AddEditUser(model);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _userService.Verify(s => s.Update(It.Is<User>(u => u.Id == user.Id)), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>();
    }

    [Fact]
    public void ViewDeleteUser_Get_WithValidId_ReturnsViewWithModel()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().Last();
        _userService.Setup(s => s.GetById(user.Id)).Returns(new[] { user });

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.ViewDeleteUser(user.Id, true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<ViewResult>();
        var model = (result as ViewResult)!.Model;
        model.Should().BeOfType<UserDetailsReadOnlyViewModel>();
        ((UserDetailsReadOnlyViewModel)model).Id.Should().Be(user.Id);
        ((UserDetailsReadOnlyViewModel)model).IsDelete.Should().BeTrue();
    }

    [Fact]
    public void ViewDeleteUser_Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        _userService.Setup(s => s.GetById(999)).Returns(Enumerable.Empty<User>());

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.ViewDeleteUser(999, true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void DeleteUser_Post_CallsDeleteAndRedirects()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var user = SetupUsers().Last();
        _userService.Setup(s => s.GetById(user.Id)).Returns(new[] { user });
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
        var result = controller.DeleteUser(model);

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

        if (allUsers)
        {
            _userService
                .Setup(s => s.GetAll())
                .Returns(users);
        }
        else
        {
            _userService
                .Setup(s => s.FilterByActive(isActive))
                .Returns(users);
        }

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
