using DrakthulJelita.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DrakthulJelita.Web.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Screenshot> Screenshots { get; set; }

    public virtual DbSet<WowClass> WowClasses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WowClass>().HasData(
            new WowClass
            {
                Id = 1, Name = "death knight", Color = "#C41E3A",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 2, Name = "demon hunter", Color = "#A330C9",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 3, Name = "druid", Color = "#FF7C0A",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 4, Name = "evoker", Color = "#33937F",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 5, Name = "hunter", Color = "#AAD372",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 6, Name = "mage", Color = "#3FC7EB",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 7, Name = "monk", Color = "#00FF98",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 8, Name = "paladin", Color = "#F48CBA",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 9, Name = "priest", Color = "#FFFFFF",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 10, Name = "rogue", Color = "#FFF468",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 11, Name = "shaman", Color = "#0070DD",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 12, Name = "warlock", Color = "#8788EE",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            },
            new WowClass
            {
                Id = 13, Name = "warrior", Color = "#C69B6D",
                CreatedAt = new DateTime(2025, 4, 12, 9, 5, 46, DateTimeKind.Utc)
            }
        );
    }
}