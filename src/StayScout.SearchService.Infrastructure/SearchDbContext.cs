using Microsoft.EntityFrameworkCore;
using StayScout.SearchService.Domain;

namespace StayScout.SearchService.Infrastructure;

public sealed class SearchDbContext(
    DbContextOptions<SearchDbContext> options) : DbContext(options)
{
    public DbSet<Search> Searches => Set<Search>();

    public DbSet<SearchResult> SearchResults => Set<SearchResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Search>(entity =>
        {
            entity.HasKey(search => search.Id);

            entity.Property(search => search.Query)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasIndex(search => new
            {
                search.Status,
                search.CreatedAt
            });
        });

        modelBuilder.Entity<SearchResult>(entity =>
        {
            entity.HasKey(result => result.Id);

            entity.Property(result => result.ProviderResultId)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(result => result.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(result => result.City)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(result => result.Address)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(result => result.PricePerNight)
                .HasPrecision(18, 2);

            entity.HasIndex(result => new
            {
                result.SearchId,
                result.ProviderResultId
            }).IsUnique();

            entity.HasOne<Search>()
                .WithMany()
                .HasForeignKey(result => result.SearchId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
