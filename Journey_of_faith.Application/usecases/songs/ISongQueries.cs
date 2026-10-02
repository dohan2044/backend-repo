using Journey_of_faith.Application.common.dtos.songs;
using Journey_of_faith.Domain.dtos;

namespace Journey_of_faith.Application.usecases.songs;

public interface ISongQueries
{
    Task<bool> SongCategoryExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> SongCategoryExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> SongExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> SongCategoryMappingExistsAsync(int songId, int categoryId, CancellationToken cancellationToken = default);
    Task<bool> ArtistExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ArtistNameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> AlbumExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<SongDto>> GetSongsAsync(SongFilterDto filter, CancellationToken cancellationToken = default);
    Task<SongDto?> GetSongAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SongCategoryDto>> GetSongCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SongCategoryMappingDto>> GetSongCategoryMappingsAsync(int? songId, int? categoryId, CancellationToken cancellationToken = default);
}
