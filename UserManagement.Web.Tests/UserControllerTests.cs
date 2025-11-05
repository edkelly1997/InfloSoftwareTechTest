using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using System.Linq;

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
        var controller = CreateController();
        var users = SetupUsers(allUsers: false, isActive: true);

        var result = controller.List(allUsers: false, isActive: true);

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_InactiveUsers_ModelMustContainOnlyInactiveUsers()
    {
        var controller = CreateController();
        var users = SetupUsers(allUsers: false, isActive: false);

        var result = controller.List(allUsers: false, isActive: false);

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    private User[] SetupUsers(bool allUsers = true, bool isActive = true)
    {

        var ActiveUser = new[]
        {
            new User
            {
                Forename = "Johnny",
                Surname = "User",
                Email = "juser@example.com",
                IsActive = true
            }
        };

        var InActiveUser = new[]
        {
            new User
            {
                Forename = "Fred",
                Surname = "IsInactive",
                Email = "fisinactive@test.com",
                IsActive = false
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
