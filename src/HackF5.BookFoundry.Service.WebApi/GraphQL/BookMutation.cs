namespace HackF5.BookFoundry.Service.WebApi.GraphQL;

using Articulum.Core.GraphQL;

using HackF5.BookFoundry.Service.WebApi.Data;
using HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

using HotChocolate.Resolvers;

[ExtendObjectType(typeof(RootMutation))]
public class BookMutation
{
    public async Task<CreateBookOutput> CreateBookAsync(
        CreateBookInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var book = ctx.Books.AddProxy();
            book.Name = input.Name;

            var entry = ctx.Entries.AddProxy();
            entry.Name = "Entry 1";
            entry.Book = book;

            var revision = ctx.Revisions.AddProxy();
            revision.Active = true;
            revision.Entry = entry;
            revision.Text = string.Empty;

            await ctx.SaveChangesAsync(cancellation);

            return new(book.Id);
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while creating the book.", ex, context);
        }
    }

    public async Task<CreateEntryOutput> CreateEntryAsync(
        CreateEntryInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var previousEntry = await ctx.Entries.FirstAsync(x => x.Id == input.PreviousEntryId, cancellation);
            var entry = ctx.Entries.AddProxy();
            entry.Name = input.Name;
            entry.NextEntryId = previousEntry.NextEntryId;
            previousEntry.NextEntry = entry;
            entry.Book = previousEntry.Book;

            var revision = ctx.Revisions.AddProxy();
            revision.Active = true;
            revision.Entry = entry;
            revision.Text = string.Empty;

            await ctx.SaveChangesAsync(cancellation);

            return new(entry.Id);
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while creating the entry.", ex, context);
        }
    }
}