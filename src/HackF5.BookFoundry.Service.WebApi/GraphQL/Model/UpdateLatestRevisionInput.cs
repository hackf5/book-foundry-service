namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public class UpdateLatestRevisionInput
{
    public int EntryId { get; set; }

    public uint ConcurrencyToken { get; set; }

    public string Text { get; set; } = default!;
}