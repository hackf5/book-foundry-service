namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public class UpdateEntryInput
{
    public int EntryId { get; set; }
    
    public string Name { get; set; } = default!;
}