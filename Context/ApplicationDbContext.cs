using Microsoft.EntityFrameworkCore;
using LogistTrans.Models;
using Route = LogistTrans.Models.Route;

namespace LogistTrans.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }
    
    public DbSet<Client> Clients { get; set; }
    public DbSet<Employee> Employees { get; set; }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<Notification> Notifications { get; set; }
    
    public DbSet<Route> Routes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связей
        modelBuilder.Entity<UserLogin>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<Client>()
            .HasOne(c => c.Login)
            .WithMany()
            .HasForeignKey(c => c.LoginId);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Login)
            .WithMany()
            .HasForeignKey(e => e.LoginId);

        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => new { oi.OrderId, oi.ProductId });
        
        modelBuilder.Entity<Route>()
            .HasOne(r => r.Order)
            .WithMany()
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Route>()
            .HasOne(r => r.Employee)
            .WithMany()
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}