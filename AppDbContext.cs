using daandemaat.github.io.Models;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Project> Projecten => Set<Project>();
}