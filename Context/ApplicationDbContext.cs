using Microsoft.EntityFrameworkCore;
using LogistTrans.Models;

namespace LogistTrans.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Employee> Employees { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Дополнительные настройки связей, если необходимо
    }
}