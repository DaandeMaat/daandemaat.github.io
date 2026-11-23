using daandemaat.github.io.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Project> Projecten => Set<Project>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<User> Users => Set<User>();
}