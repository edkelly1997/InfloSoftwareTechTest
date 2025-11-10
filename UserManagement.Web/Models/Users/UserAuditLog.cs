using System;

namespace UserManagement.Web.Models.Users;

public class UserAuditLogListViewModel
{
    public List<UserAuditLogViewModel> Items { get; set; } = new();
}

public class UserAuditLogViewModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = default!;
    public string ChangeType { get; set; } = default!;
    public DateTime ChangedAt { get; set; }
    public string ChangedData { get; set; } = default!;
    public string OldDetails { get; set; } = default!;
    public string NewDetails { get; set; } = default!;
}
