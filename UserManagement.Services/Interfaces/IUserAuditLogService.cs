using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserAuditLogService 
{
    Task<IEnumerable<UserAuditLog?>> GetAllById(long id);
    Task<IEnumerable<UserAuditLog>> GetAll();
    Task<IEnumerable<UserAuditLog?>> GetAllByUserId(long id);
    Task Create(UserAuditLog auditLog);
}
