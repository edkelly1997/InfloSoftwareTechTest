using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<User>().HasData(new[]
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", IsActive = true, DateOfBirth = DateOnly.Parse("08/01/1968") },
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", IsActive = true, DateOfBirth = DateOnly.Parse("20/03/1997") },
            new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", IsActive = false, DateOfBirth = DateOnly.Parse("05/04/1990") },
            new User { Id = 4, Forename = "Memphis", Surname = "Raines", Email = "mraines@example.com", IsActive = true, DateOfBirth = DateOnly.Parse("17/08/1960") },
            new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", Email = "sgodspeed@example.com", IsActive = true, DateOfBirth = DateOnly.Parse("24/12/1991") },
            new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", Email = "himcdunnough@example.com", IsActive = true, DateOfBirth = DateOnly.Parse("25/08/1994") },
            new User { Id = 7, Forename = "Cameron", Surname = "Poe", Email = "cpoe@example.com", IsActive = false, DateOfBirth = DateOnly.Parse("04/05/1998") },
            new User { Id = 8, Forename = "Edward", Surname = "Malus", Email = "emalus@example.com", IsActive = false, DateOfBirth = DateOnly.Parse("15/02/1986") },
            new User { Id = 9, Forename = "Damon", Surname = "Macready", Email = "dmacready@example.com", IsActive = false, DateOfBirth = DateOnly.Parse("02/02/2002") },
            new User { Id = 10, Forename = "Johnny", Surname = "Blaze", Email = "jblaze@example.com", IsActive = true, DateOfBirth = DateOnly.Parse("12/06/1975") },
            new User { Id = 11, Forename = "Robin", Surname = "Feld", Email = "rfeld@example.com", IsActive = true, DateOfBirth = DateOnly.Parse("11/11/2011") },
        });
    }

    public DbSet<User>? Users { get; set; }

    public async Task<IQueryable<TEntity>> GetAll<TEntity>() where TEntity : class
    {
        return await Task.FromResult(Set<TEntity>());
    }

    public async Task Create<TEntity>(TEntity entity) where TEntity : class
    {
        await Set<TEntity>().AddAsync(entity);
        await SaveChangesAsync();
    }

    public new async Task Update<TEntity>(TEntity entity) where TEntity : class
    {
        Set<TEntity>().Update(entity);
        await SaveChangesAsync();
    }

    public async Task Delete<TEntity>(TEntity entity) where TEntity : class
    {
        Set<TEntity>().Remove(entity);
        await SaveChangesAsync();
    }
}
