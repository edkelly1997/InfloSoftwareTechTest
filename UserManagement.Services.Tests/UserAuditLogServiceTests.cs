using System.Linq;
using System.Threading.Tasks;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Data.Tests;

public class UserAuditLogServiceTests
{
    [Fact]
    public async Task GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var logs = SetUpLogs();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public async Task GetById_WhenIdExists_MustReturnMatchingLog()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var logs = SetUpLogs();
        var expectedLog = logs.First();
        
        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAllById(expectedLog.Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var actualLog = result.FirstOrDefault();
        actualLog.Should().NotBeNull();
        actualLog.Should().BeEquivalentTo(expectedLog);
    }

    [Fact]
    public async Task GetById_WhenIdDoesNotExist_MustReturnEmpty()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var nonExistentId = 999;

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAllById(nonExistentId);

        // Assert: Verifies that the action of the method under test behaves as expected.
        var actualLog = result.FirstOrDefault();
        actualLog.Should().BeNull();
    }

    [Fact]
    public async Task Create_WhenCalled_MustInvokeDataContextCreate()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var newLog =
            new UserAuditLog
            {
                Id = 1,
                ChangeType = "Created",
                UserFullName = "Ed Kelly",
                        ChangedData = @"{
            ""OldDetails"": { ""Id"": 0, ""Forename"": """", ""Surname"": """", ""Email"": """", ""IsActive"": false, ""DateOfBirth"": null },
            ""NewDetails"": { ""Id"": 1, ""Forename"": ""Ed"", ""Surname"": ""Kelly"", ""Email"": ""ed.kelly@example.com"", ""IsActive"": true, ""DateOfBirth"": ""1990-05-12"" }
        }"
        };
        var service = CreateService();

        // Act: Invokes the method under test with the arranged parameters.
        await service.Create(newLog);

        // Assert: Verifies that the action of the method under test behaves as expected.
        _dataContext.Verify(dc => dc.Create(newLog), Times.Once);
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

    private readonly Mock<IDataContext> _dataContext = new();
    private readonly Mock<IUserAuditLogService> _userAuditLogService = new();
    private UserAuditLogService CreateService()
    {
        var logs = SetUpLogs().AsQueryable();
        _dataContext.Setup(s => s.GetAll<UserAuditLog>()).ReturnsAsync(logs);
        return new UserAuditLogService(_dataContext.Object);
    }

}
