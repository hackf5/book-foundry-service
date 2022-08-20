namespace HackF5.BookFoundry.Service.WebApi.Data;

[Index(nameof(Name), IsUnique = true)]
public class TagEntity : NamedEntityBase
{
    public virtual ICollection<BookEntryTagEntity> Entries { get; } = new HashSet<BookEntryTagEntity>();
}