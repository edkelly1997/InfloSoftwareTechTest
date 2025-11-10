using System.Linq;
using System.Threading.Tasks;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using System.Text.Json;
using System.Text;

namespace UserManagement.WebMS.Controllers;

public class LogsController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserAuditLogService _userAuditLogService;
    public LogsController(IUserService userService, IUserAuditLogService userAuditLogService)
    {
        _userService = userService;
        _userAuditLogService = userAuditLogService;
    }

    [HttpGet]
    public async Task<ViewResult> List(long userId = 0, bool allLogs = true)
    {
        IEnumerable<UserAuditLogViewModel> items;

        if (allLogs)
        {
            var logs = await _userAuditLogService.GetAll();
            items = logs.Select(log => new UserAuditLogViewModel
            {
                Id = log.Id,
                UserId = log.UserId,
                UserFullName = log.UserFullName,
                ChangeType = log.ChangeType,
                ChangedAt = log.ChangedAt,
                ChangedData = log.ChangedData
            });
        }
        else
        {
            var logs = await _userAuditLogService.GetAllByUserId(userId);
            items = logs.Select(log => new UserAuditLogViewModel
            {
                Id = log!.Id,
                UserId = log.UserId,
                UserFullName = log.UserFullName,
                ChangeType = log.ChangeType,
                ChangedAt = log.ChangedAt,
                ChangedData = log.ChangedData
            });
        }
        var model = new UserAuditLogListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ViewAuditLog(long id)
    {
        UserAuditLogViewModel log = new UserAuditLogViewModel();

        var logsById = await _userAuditLogService.GetAllById(id);
        var logEntry = logsById.FirstOrDefault();

        if (logEntry != null)
        {
            var changeDetails = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(logEntry.ChangedData);

            log.Id = logEntry.Id;
            log.UserId = logEntry.UserId;
            log.UserFullName = logEntry.UserFullName;
            log.ChangeType = logEntry.ChangeType;
            log.ChangedAt = logEntry.ChangedAt;
            log.ChangedData = logEntry.ChangedData;
            log.OldDetails = FlattenJsonElement(changeDetails?["OldDetails"]);
            log.NewDetails = FlattenJsonElement(changeDetails?["NewDetails"]);
        }
        else
        {
            return NotFound();
        }

        return View(log);
    }

    private string FlattenJsonElement(JsonElement? element)
    {
        if (element == null || element.Value.ValueKind != JsonValueKind.Object)
            return string.Empty;

        var sb = new StringBuilder();
        foreach (var prop in element.Value.EnumerateObject())
        {
            sb.Append($"{prop.Name}: {prop.Value.GetRawText()} ");
        }
        return sb.ToString().Trim();
    }
}
