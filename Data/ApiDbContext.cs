using Microsoft.EntityFrameworkCore;
using MVCcrypto.Models.Models;

namespace MVCcrypto.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options):base(options) {  }     
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Currency>().Property(c => c.AddedDate).HasDefaultValueSql("now()");
    }
    public virtual DbSet<Currency>? Currencies { get; set; }
}