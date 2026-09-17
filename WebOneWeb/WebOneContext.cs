using Microsoft.EntityFrameworkCore;
using Throw;
using WebOneCore;

namespace WebOneWeb;

public class WebOneDbContext(DbContextOptions<WebOneDbContext> options) : DbContext(options)
{
    public virtual DbSet<Contact> Contacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbConnString = Environment.GetEnvironmentVariable("WEBONE_DB_CONNECTION_STRING");
        dbConnString.ThrowIfNull("WEBONE_DB_CONNECTION_STRING must be defined in the environment").IfEmpty();
        optionsBuilder.UseNpgsql(dbConnString);

        optionsBuilder.UseAsyncSeeding(async (context, _, token) => 
        {
            var firstContact = await context.Set<Contact>().FirstOrDefaultAsync(token);
            if (firstContact is null) 
            {
                var defaultContact = new Contact {
                    Id = 1,
                    Name = "Spencer",
                    Email = "spencernaugler7@gmail.com"
                };
                await context.AddAsync(defaultContact, token);
                await context.SaveChangesAsync(token);
            }
        });
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
