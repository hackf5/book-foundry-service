namespace HackF5.BookFoundry.Service.WebApi.Data;

using System.ComponentModel.DataAnnotations.Schema;

[Index(nameof(BookEntryId), nameof(TagId), IsUnique = true)]
public class BookEntryTagEntity : EntityBase
{
    [ForeignKey(nameof(Entry))]
    public int BookEntryId { get; set; }

    public virtual BookEntryEntity Entry { get; set; } = default!;

    [ForeignKey(nameof(Tag))]
    public int TagId { get; set; }

    public virtual TagEntity Tag { get; set; } = default!;
}