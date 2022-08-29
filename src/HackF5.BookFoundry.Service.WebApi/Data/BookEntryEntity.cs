namespace HackF5.BookFoundry.Service.WebApi.Data;

using System.ComponentModel.DataAnnotations.Schema;

[Index(nameof(BookId), nameof(Name), IsUnique = true)]
[Index(nameof(BookId), nameof(Index), IsUnique = true)]
public class BookEntryEntity : NamedEntityBase
{
    [ForeignKey(nameof(Book))]
    public int BookId { get; set; }

    public virtual BookEntity Book { get; set; } = default!;

    public int Index { get; set; }

    public DateTime? Deleted { get; set; }

    public virtual ICollection<BookEntryRevisionEntity> Revisions { get; } = new HashSet<BookEntryRevisionEntity>();

    public virtual ICollection<BookEntryTagEntity> Tags { get; } = new HashSet<BookEntryTagEntity>();

    public BookEntryEntity? Next() => this.Book.Entries
        .OrderBy(x => x.Index)
        .SkipWhile(x => x.Index <= this.Index)
        .FirstOrDefault();

    public BookEntryEntity? Previous() => this.Book.Entries
        .OrderByDescending(x => x.Index)
        .SkipWhile(x => x.Index >= this.Index)
        .FirstOrDefault();

    public BookEntryRevisionEntity LatestRevision() => this.Revisions.OrderByDescending(x => x.Id).First();
}