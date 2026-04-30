using Microsoft.EntityFrameworkCore;
using NextInLine.Models;

namespace NextInLine.Data;

public class MysqlDbContext:DbContext
{
    public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Ticket> tickets  { get; set; }
    public DbSet<User> users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Status)
            .HasConversion<string>();
    }
}