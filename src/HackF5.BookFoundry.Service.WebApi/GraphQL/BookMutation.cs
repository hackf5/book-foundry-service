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
        catch (QueryException)
        {
            throw;
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
            var book = await ctx.Books.FirstAsync(x => x.Id == input.BookId, cancellation);

            var entry = ctx.Entries.AddProxy();
            entry.Name = input.Name;
            entry.Index = book.Entries.Max(x => x.Index) + 1;
            entry.Book = book;

            var revision = ctx.Revisions.AddProxy();
            revision.Entry = entry;
            revision.Text = string.Empty;

            await ctx.SaveChangesAsync(cancellation);

            return new(entry.Id);
        }
        catch (QueryException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while creating the entry.", ex, context);
        }
    }

    public async Task<CreateEntryOutput> DeleteEntryAsync(
        DeleteEntryInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var entry = await ctx.Entries.FirstAsync(x => x.Id == input.EntryId, cancellation);

            AssertEntryNotDeleted(context, entry);

            entry.Deleted = DateTime.UtcNow;

            await ctx.SaveChangesAsync(cancellation);

            return new(entry.Id);
        }
        catch (QueryException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while creating the entry.", ex, context);
        }
    }

    public async Task<CreateEntryOutput> RestoreEntryAsync(
        RestoreEntryInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var entry = await ctx.Entries.FirstAsync(x => x.Id == input.EntryId, cancellation);

            if (entry.Deleted is null)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                    .SetMessage($"The entry is not deleted.")
                    .SetCode($"-{nameof(entry.Deleted)}")
                    .SetPath(context.Path)
                    .Build());
            }

            entry.Deleted = null;

            await ctx.SaveChangesAsync(cancellation);

            return new(entry.Id);
        }
        catch (QueryException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while creating the entry.", ex, context);
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

            AssertEntryNotDeleted(context, entry);

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
        catch (QueryException)
        {
            throw;
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

            AssertEntryNotDeleted(context, entry);

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

    private static void AssertEntryNotDeleted(IResolverContext context, BookEntryEntity entry)
    {
        if (entry.Deleted is not null)
        {
            throw new QueryException(
                ErrorBuilder.New()
                .SetMessage($"The entry was deleted at {entry.Deleted:o}.")
                .SetCode(nameof(entry.Deleted))
                .SetPath(context.Path)
                .Build());
        }
    }
}