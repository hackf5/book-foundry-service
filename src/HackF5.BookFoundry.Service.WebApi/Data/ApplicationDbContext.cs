namespace HackF5.BookFoundry.Service.WebApi.Data;

public class ApplicationDbContext : ApplicationDbContextBase
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}