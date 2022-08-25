namespace HackF5.BookFoundry.Service.WebApi.GraphQL;

using Articulum.Core.GraphQL;

using HackF5.BookFoundry.Service.WebApi.Data;
using HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

using HotChocolate.Execution;
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
            entry.Index = 0;
            entry.Book = book;

            var revision = ctx.Revisions.AddProxy();
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

    public async Task<CreateRevisionOutput> CreateRevisionAsync(
        CreateRevisionInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var entry = await ctx.Entries.FirstAsync(x => x.Id == input.EntryId, cancellation);

            var revision = ctx.Revisions.AddProxy();
            revision.Entry = entry;
            revision.Text = input.Text;

            await ctx.SaveChangesAsync(cancellation);

            return new(
                revision.Entry.BookId,
                revision.EntryId,
                revision.Id,
                revision.ConcurrencyToken);
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while creating the revision.", ex, context);
        }
    }

    public async Task<UpdateLatestRevisionOutput> UpdateLatestRevisionAsync(
        UpdateLatestRevisionInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var entry = await ctx.Entries.FirstAsync(x => x.Id == input.EntryId, cancellation);
            var revision = entry.LatestRevision;

            if (revision.ConcurrencyToken != input.ConcurrencyToken)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                    .SetMessage($"Concurrency token mismatch when updating revision {revision.Id}")
                    .SetCode(nameof(revision.ConcurrencyToken))
                    .SetPath(context.Path)
                    .Build());
            }

            revision.Text = input.Text;

            await ctx.SaveChangesAsync(cancellation);

            return new(
                revision.Entry.BookId,
                revision.EntryId,
                revision.Id,
                revision.ConcurrencyToken);
        }
        catch (QueryException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while updating the revision.", ex, context);
        }
    }
}