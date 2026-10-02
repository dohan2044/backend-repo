namespace Journey_of_faith.Domain.entities.catholic;
public class BibleVerse
{
    public int Id { get; set; }

    public int ChapterId { get; set; }

    public int VerseNumber { get; set; }

    public string TextVi { get; set; } = null!;

    public string? TextEn { get; set; }

    public BibleChapter Chapter { get; set; } = null!;
}