namespace HackF5.BookFoundry.Service.WebApi.GraphQL.Model;

public class CreateBookOutput
{
    public CreateBookOutput(int bookId) => this.BookId = bookId;

    public int BookId { get; }
}