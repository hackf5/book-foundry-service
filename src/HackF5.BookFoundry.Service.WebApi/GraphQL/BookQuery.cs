namespace HackF5.BookFoundry.Service.WebApi.GraphQL;

using Articulum.Core.GraphQL;

using HackF5.BookFoundry.Service.WebApi.Data;

#pragma warning disable CA1822 // mark member as static

[ExtendObjectType(typeof(RootQuery))]
public class BookQuery
{
    public IQueryable<BookEntity> GetBooks([Service] ApplicationDbContext ctx) => ctx.Books;
}