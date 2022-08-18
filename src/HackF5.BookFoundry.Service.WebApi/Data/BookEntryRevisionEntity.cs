namespace HackF5.BookFoundry.Service.WebApi.Data;

using System.ComponentModel.DataAnnotations.Schema;

public class BookEntryRevisionEntity : EntityBase
{
    public string Text { get; set; } = default!;

    [ForeignKey(nameof(Entry))]
    public int EntryId { get; set; }

    public virtual BookEntryEntity Entry { get; set; } = default!;

    public BookEntryRevisionEntity? PreviousRevision => this.Entry.Revisions
        .OrderByDescending(x => x.Id)
        .FirstOrDefault(x => x.Id < this.Id);

    public BookEntryRevisionEntity? NextRevision => this.Entry.Revisions
        .OrderBy(x => x.Id)
        .FirstOrDefault(x => x.Id > this.Id);
}