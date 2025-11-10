using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserAuditLogService : IUserAuditLogService
{
    private readonly IDataContext _dataAccess;
    public UserAuditLogService(IDataContext dataAccess) => _dataAccess = dataAccess;

    public async Task<IEnumerable<UserAuditLog>> GetAll()
    {
        var auditLogs = await _dataAccess.GetAll<UserAuditLog>();
        return auditLogs.ToList();
    }

    public async Task<IEnumerable<UserAuditLog?>> GetAllById(long id)
    {
        var auditLogs = await _dataAccess.GetAll<UserAuditLog>();
        return auditLogs.ToList().Where(a => a.Id == id);
    }

    public async Task<IEnumerable<UserAuditLog?>> GetAllByUserId(long id)
    {
        var auditLogs = await _dataAccess.GetAll<UserAuditLog>();
        return auditLogs.ToList().Where(a => a.UserId == id);
    }

    public Task Create(UserAuditLog auditLog)
    {
        if (auditLog == null) throw new ArgumentNullException(nameof(auditLog));
        return _dataAccess.Create(auditLog);
    }
}
