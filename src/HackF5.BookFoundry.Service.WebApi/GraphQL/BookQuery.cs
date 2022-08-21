namespace HackF5.BookFoundry.Service.WebApi.GraphQL;

using Articulum.Core.GraphQL;

using HackF5.BookFoundry.Service.WebApi.Data;

using HotChocolate.Data;

#pragma warning disable CA1822 // mark member as static

[ExtendObjectType(typeof(RootQuery))]
public class BookQuery
{
    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseFiltering]
    [UseSorting]
    public IQueryable<BookEntity> GetBooks([Service] ApplicationDbContext ctx) => ctx.Books;

    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseFiltering]
    [UseSorting]
    public IQueryable<BookEntryEntity> GetEntries([Service] ApplicationDbContext ctx) => ctx.Entries;

    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseFiltering]
    [UseSorting]
    public IQueryable<BookEntryRevisionEntity> GetRevisions([Service] ApplicationDbContext ctx) => ctx.Revisions;
}