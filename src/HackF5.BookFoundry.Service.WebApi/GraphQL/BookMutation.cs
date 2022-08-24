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

    public async Task<SetPreviousEntryOutput> SetPreviousEntryAsync(
        SetPreviousEntryInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var entry = await ctx.Entries.FirstAsync(x => x.Id == input.EntryId, cancellation);
            var previousEntry = await ctx.Entries.FirstAsync(x => x.Id == input.PreviousEntryId, cancellation);

            if (entry.Book != previousEntry.Book)
            {
                throw new InvalidOperationException("Book mismatch.");
            }

            if (entry.PreviousEntry != null)
            {
                entry.PreviousEntry.NextEntry = entry.NextEntry;
            }

            entry.NextEntry = previousEntry.NextEntry;
            previousEntry.NextEntry = entry;

            await ctx.SaveChangesAsync(cancellation);

            return new(entry.Id);
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while setting the previous entry.", ex, context);
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
            foreach (var r in entry.Revisions)
            {
                r.Active = false;
            }

            var revision = ctx.Revisions.AddProxy();
            revision.Active = true;
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

    public async Task<ActivateRevisionOutput> ActivateRevisionAsync(
        ActivateRevisionInput input,
        [Service] ApplicationDbContext ctx,
        IResolverContext context,
        CancellationToken cancellation)
    {
        try
        {
            var revision = await ctx.Revisions
                .Include(x => x.Entry.Revisions)
                .FirstAsync(x => x.Id == input.RevisionId, cancellation);

            foreach (var r in revision.Entry.Revisions)
            {
                r.Active = false;
            }

            revision.Active = true;

            await ctx.SaveChangesAsync(cancellation);

            return new(revision.Id);
        }
        catch (Exception ex)
        {
            throw QueryMessageException.New("An error occurred while updating the revision.", ex, context);
        }
    }
}