using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;

namespace UserManagement.Data.Tests;

public class LogsControllerTests
{
    [Fact]
    public async Task List_WhenServiceReturnsLogs_ModelMustContainAllLogs()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var logs = SetUpLogs();
        _userAuditLogService.Setup(_userAuditLogService => _userAuditLogService.GetAll()).ReturnsAsync(logs);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        var model = (result as ViewResult)!.Model;
        model.Should().BeOfType<UserAuditLogListViewModel>();
        ((UserAuditLogListViewModel)model).Items.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public async Task List_UserId_ModelMustContainOnlyLogsForUse()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var logs = SetUpLogs().Where(l => l.Id == 1);
        _userAuditLogService.Setup(_userAuditLogService => _userAuditLogService.GetAllByUserId(1)).ReturnsAsync(logs);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List(allLogs: false, userId: 1);

        var model = (result as ViewResult)!.Model;
        model.Should().BeOfType<UserAuditLogListViewModel>();
        ((UserAuditLogListViewModel)model).Items.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public async Task ViewLog_Get_ReturnsLogView()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var logs = SetUpLogs().First();
        _userAuditLogService.Setup(_userAuditLogService => _userAuditLogService.GetAllById(logs.Id)).ReturnsAsync(new List<UserAuditLog?> { logs });

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.ViewAuditLog(logs.Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model;
        model.Should().BeOfType<UserAuditLogViewModel>();
        ((UserAuditLogViewModel)model).Id.Should().Be(logs.Id);
    }

    [Fact]
    public async Task ViewLog_Get_InvalidId_ReturnsNotFound()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        _userAuditLogService.Setup(_userAuditLogService => _userAuditLogService.GetAllById(999)).ReturnsAsync(Enumerable.Empty<UserAuditLog?>());

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.ViewAuditLog(999);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeOfType<NotFoundResult>();
    }

    private UserAuditLog[] SetUpLogs(bool allLogs = true, long userId = 0)
    {

        var LogId1 = new[]
        {
            new UserAuditLog
            {
                Id = 1,
                ChangeType = "Created",
                UserFullName = "Ed Kelly",
                        ChangedData = @"{
            ""OldDetails"": { ""Id"": 0, ""Forename"": """", ""Surname"": """", ""Email"": """", ""IsActive"": false, ""DateOfBirth"": null },
            ""NewDetails"": { ""Id"": 1, ""Forename"": ""Ed"", ""Surname"": ""Kelly"", ""Email"": ""ed.kelly@example.com"", ""IsActive"": true, ""DateOfBirth"": ""1990-05-12"" }
        }"
            }
        };

        var LogId2 = new[]
        {
            new UserAuditLog
            {
                Id = 2,
                ChangeType = "Created",
                UserFullName = "Ed Kelly",
                        ChangedData = @"{
            ""OldDetails"": { ""Id"": 2, ""Forename"": ""Jane"", ""Surname"": ""Doe"", ""Email"": ""jane.old@example.com"", ""IsActive"": true, ""DateOfBirth"": ""1985-08-21"" },
            ""NewDetails"": { ""Id"": 2, ""Forename"": ""Jane"", ""Surname"": ""Doe"", ""Email"": ""jane.doe@example.com"", ""IsActive"": true, ""DateOfBirth"": ""1985-08-21"" }
        }",
            }
        };

        UserAuditLog[] logs = LogId1.Concat(LogId2).ToArray();

        return logs;
    }

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IUserAuditLogService> _userAuditLogService = new();
    private LogsController CreateController() => new(_userService.Object, _userAuditLogService.Object);
}
