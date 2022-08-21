namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public record CreateRevisionOutput(
    int BookId,
    int EntryId,
    int RevisionId);