namespace Journey_of_faith.Domain.entities.catholic;
public class BibleBook
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public string Testament { get; set; } = null!;

    public string? Category { get; set; }

    public int ChapterCount { get; set; }

    public ICollection<BibleChapter> Chapters { get; set; }
        = new List<BibleChapter>();
}