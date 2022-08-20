namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public class SetPreviousEntryInput
{
    public int EntryId { get; set; }

    public int PreviousEntryId { get; set; }
}