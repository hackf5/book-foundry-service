namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public record MoveEntryOutput(
    int BookId,
    int EntryId,
    int OldIndex,
    int NewIndex);