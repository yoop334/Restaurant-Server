using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Repository;

public class DataContext : DbContext
{
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<User>()
    //         .HasMany(user => user.)
    // }
}