using System;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Fact]
    public void FilterByActive_WhenContextReturnsEntities_MustReturnActiveEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers().Where(u => u.IsActive).ToArray();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.FilterByActive(true);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Fact]
    public void FilterByInactive_WhenContextReturnsEntities_MustReturnInactiveEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers().Where(u => u.IsActive).ToArray();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.FilterByActive(false);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Fact]
    public void GetById_WhenIdExists_MustReturnMatchingUser()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();
        var expectedUser = users.First();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetById(expectedUser.Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public void GetById_WhenIdDoesNotExist_MustReturnEmpty()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var nonExistentId = 999;

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetById(nonExistentId);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEmpty();
    }

    [Fact]
    public void Create_WhenCalled_MustInvokeDataContextCreate()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var user = new User
        {
            Id = 3,
            Forename = "Alice",
            Surname = "New",
            Email = "alice.new@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.Parse("02/02/2002")
        };
        var service = CreateService();

        // Act: Invokes the method under test with the arranged parameters.
        service.Create(user);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _dataContext.Verify(dc => dc.Create(user), Times.Once);
    }

    [Fact]
    public void Update_WhenCalled_MustInvokeDataContextUpdate()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var user = new User
        {
            Id = 1,
            Forename = "Johnny",
            Surname = "User",
            Email = "juser@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.Parse("01/01/2001")
        };
        var service = CreateService();

        // Act: Invokes the method under test with the arranged parameters.
        service.Update(user);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _dataContext.Verify(dc => dc.Update(user), Times.Once);
    }

    [Fact]
    public void Delete_WhenCalled_MustInvokeDataContextDelete()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var user = new User
        {
            Id = 2,
            Forename = "Fred",
            Surname = "IsInactive",
            Email = "fisinactive@test.com",
            IsActive = false,
            DateOfBirth = DateOnly.Parse("01/01/2001")
        };
        var service = CreateService();

        // Act: Invokes the method under test with the arranged parameters.
        service.Delete(user);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _dataContext.Verify(dc => dc.Delete(user), Times.Once);
    }

    private User[] SetupUsers()
    {
        var activeUser = new User
        {
            Id = 1,
            Forename = "Johnny",
            Surname = "User",
            Email = "juser@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.Parse("01/01/2001")
        };

        var inactiveUser = new User
        {
            Id = 2,
            Forename = "Fred",
            Surname = "IsInactive",
            Email = "fisinactive@test.com",
            IsActive = false,
            DateOfBirth = DateOnly.Parse("01/01/2001")
        };

        return new[] { activeUser, inactiveUser };
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private readonly Mock<IUserService> _userService = new();
    private UserService CreateService()
    {
        var users = SetupUsers().AsQueryable();
        _dataContext.Setup(s => s.GetAll<User>()).Returns(users);
        return new UserService(_dataContext.Object);
    }

}
