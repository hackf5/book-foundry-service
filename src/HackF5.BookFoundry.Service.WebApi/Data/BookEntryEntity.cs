namespace HackF5.BookFoundry.Service.WebApi.Data;

using System.ComponentModel.DataAnnotations.Schema;

[Index(nameof(Name), IsUnique = true)]
public class BookEntryEntity : NamedEntityBase
{
    [ForeignKey(nameof(Book))]
    public int BookId { get; set; }

    public virtual BookEntity Book { get; set; } = default!;

    [ForeignKey(nameof(NextEntry))]
    public int? NextEntryId { get; set; }

    public virtual BookEntryEntity? NextEntry { get; set; }

    public BookEntryEntity? PreviousEntry => this.Book.Entries.FirstOrDefault(x => x.NextEntry == this);

    public virtual ICollection<BookEntryRevisionEntity> Revisions { get; } = new HashSet<BookEntryRevisionEntity>();

    public virtual ICollection<TagEntity> Tags { get; } = new HashSet<TagEntity>();
}