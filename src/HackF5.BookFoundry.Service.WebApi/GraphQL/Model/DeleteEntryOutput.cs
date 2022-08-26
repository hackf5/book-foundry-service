namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public record DeleteEntryOutput(
    int BookId,
    int EntryId);