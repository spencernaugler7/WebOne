using Microsoft.EntityFrameworkCore;
using Throw;

namespace WebOne.Models;

public class WebOneDbContext(DbContextOptions<WebOneDbContext> options) : DbContext(options)
{
    public virtual DbSet<Contact> Contacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbConnString = Environment.GetEnvironmentVariable("WEBONE_DB_CONNECTION_STRING");
        dbConnString.ThrowIfNull("WEBONE_DB_CONNECTION_STRING must be defined in the enviornment").IfEmpty();
        optionsBuilder.UseNpgsql(dbConnString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(cb =>
        {
            cb.Property(k => k.Id)
                .UseIdentityByDefaultColumn();
            cb.Property(k => k.Name);
            cb.Property(k => k.Email);
            cb.HasKey(k => k.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}

public class Contact
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}