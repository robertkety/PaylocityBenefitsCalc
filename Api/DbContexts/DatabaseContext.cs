using Api.Models;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<Dependent> Dependents { get; set; }
    public DbSet<Employee> Employees { get; set; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //    => options.UseSqlServer("Server=(local);Database=Database;TrustServerCertificate=true;Integrated Security=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Dependent>()
            .Property(p => p.IsPartner)
            .HasComputedColumnSql("CAST(CASE WHEN Relationship = 1 OR Relationship = 2 THEN 1 ELSE 0 END AS BIT)");

        modelBuilder.Entity<Dependent>()
            .HasIndex(i => new { i.EmployeeId, i.IsPartner })
            .HasFilter("Relationship IN (1, 2)")
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .Property(p => p.Salary)
            .HasColumnType("decimal(18,2)");
    }
}