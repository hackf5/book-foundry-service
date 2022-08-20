namespace HackF5.BookFoundry.Service.WebApi.Data;

[Index(nameof(Name), IsUnique = true)]
public class TagEntity : NamedEntityBase
{
    public virtual ICollection<BookEntryEntity> Entries { get; } = new HashSet<BookEntryEntity>();
}