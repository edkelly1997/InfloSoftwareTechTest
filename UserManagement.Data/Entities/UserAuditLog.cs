using System;

public class UserAuditLog
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = default!;
    public string ChangeType { get; set; } = default!;
    public DateTime ChangedAt { get; set; }
    public string ChangedData { get; set; } = default!;
}
