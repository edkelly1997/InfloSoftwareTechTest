using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> FilterByActive(bool isActive)
    {
        var users = await _dataAccess.GetAll<User>();
        return users.Where(u => u.IsActive == isActive).ToList();
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        var users = await _dataAccess.GetAll<User>();
        return users.ToList();
    }

    public async Task<User?> GetById(long id)
    {
        var users = await _dataAccess.GetAll<User>();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public Task Create(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        return _dataAccess.Create(user);
    }

    public Task Update(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        return _dataAccess.Update(user);
    }

    public Task Delete(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        return _dataAccess.Delete(user);
    }
}
