using Journey_of_faith.Domain.dtos;

namespace Journey_of_faith.Application.common.dtos.songs;

public sealed class SongCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SongCount { get; set; }
}

public sealed class SongDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public string ArtistName { get; set; } = string.Empty;
    public int? AlbumId { get; set; }
    public string? AlbumTitle { get; set; }
    public int? Duration { get; set; }
    public string? AudioUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Lyric { get; set; }
    public int PlayCount { get; set; }
    public bool IsActive { get; set; }
    public List<SongCategoryDto> Categories { get; set; } = [];
}

public sealed class SongCategoryMappingDto
{
    public int Id { get; set; }
    public int SongId { get; set; }
    public string SongTitle { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public sealed class SongFilterDto
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
