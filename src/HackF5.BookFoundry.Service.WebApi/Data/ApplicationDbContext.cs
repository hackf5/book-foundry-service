namespace HackF5.BookFoundry.Service.WebApi.Data;

public class ApplicationDbContext : ApplicationDbContextBase
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BookEntity> Books { get; set; } = default!;

    public DbSet<BookEntryEntity> Entries { get; set; } = default!;

    public DbSet<BookEntryRevisionEntity> Revisions { get; set; } = default!;

    public DbSet<TagEntity> Tags { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookEntryRevisionEntity>()
            .HasGeneratedTsVectorColumn(p => p.SearchVector, "english", p => p.Text)
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");

        modelBuilder
            .Entity<BookEntryRevisionEntity>()
            .UseXminAsConcurrencyToken();
    }
}