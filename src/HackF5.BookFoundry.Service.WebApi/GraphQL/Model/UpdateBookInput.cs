namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public class UpdateBookInput
{
    public int BookId { get; set; }

    public string Name { get; set; } = default!;
}