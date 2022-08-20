namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public class CreateEntryInput
{
    public int PreviousEntryId { get; set; }

    public string Name { get; set; } = default!;
}