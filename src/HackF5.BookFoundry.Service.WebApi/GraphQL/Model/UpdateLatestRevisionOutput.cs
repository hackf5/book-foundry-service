namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public record UpdateLatestRevisionOutput(
    int BookId,
    int EntryId,
    int RevisionId);