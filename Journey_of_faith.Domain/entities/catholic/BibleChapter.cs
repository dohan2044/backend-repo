namespace Journey_of_faith.Domain.entities.catholic;
public class BibleChapter
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public int ChapterNumber { get; set; }

    public BibleBook Book { get; set; } = null!;

    public ICollection<BibleVerse> Verses { get; set; }
        = new List<BibleVerse>();
}