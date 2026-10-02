using Dapper;
using Journey_of_faith.Application.common.dtos.songs;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.songs;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.repositories;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.persistence.queries;

public sealed class SongQueries : BaseRepository, ISongQueries
{
    public SongQueries(IDbConnectionFactory connectionFactory, IOptions<TableSchemaName> options)
        : base(connectionFactory, options) { }

    public Task<bool> SongCategoryExistsAsync(string name, CancellationToken cancellationToken = default)
        => ExistsAsync(SongRelationShip.SongCategory,
            "Name = @Name", new { Name = name }, cancellationToken);

    public Task<bool> SongCategoryExistsAsync(int id, CancellationToken cancellationToken = default)
        => ExistsAsync(SongRelationShip.SongCategory,
            "Id = @Id", new { Id = id }, cancellationToken);

    public Task<bool> SongExistsAsync(int id, CancellationToken cancellationToken = default)
        => ExistsAsync(SongRelationShip.Song,
            "Id = @Id AND ISNULL(IsDeleted, 0) = 0", new { Id = id }, cancellationToken);

    public Task<bool> SongCategoryMappingExistsAsync(int songId, int categoryId, CancellationToken cancellationToken = default)
        => ExistsAsync(SongRelationShip.SongCategoryMapping,
            "SongId = @SongId AND CategoryId = @CategoryId", new { SongId = songId, CategoryId = categoryId }, cancellationToken);

    public Task<bool> ArtistExistsAsync(int id, CancellationToken cancellationToken = default)
        => ExistsAsync(SongRelationShip.Artist,
            "Id = @Id", new { Id = id }, cancellationToken);

    public Task<bool> ArtistNameExistsAsync(string name, CancellationToken cancellationToken = default)
        => ExistsAsync(SongRelationShip.Artist,
            "Name = @Name", new { Name = name }, cancellationToken);

    public Task<bool> AlbumExistsAsync(int id, CancellationToken cancellationToken = default)
        => ExistsAsync(SongRelationShip.Album,
            "Id = @Id", new { Id = id }, cancellationToken);

    public Task<PagedResult<SongDto>> GetSongsAsync(SongFilterDto filter, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            var sql = $"""
                SELECT COUNT(DISTINCT song.Id)
                FROM [{_schemaName.Schema}].[{SongRelationShip.Song}] song
                WHERE ISNULL(song.IsDeleted, 0) = 0
                  AND (@Keyword IS NULL OR song.Title LIKE '%' + @Keyword + '%')
                  AND (@CategoryId IS NULL OR EXISTS (
                      SELECT 1 FROM [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] filterMap
                      WHERE filterMap.SongId = song.Id AND filterMap.CategoryId = @CategoryId));

                WITH PagedSongs AS (
                    SELECT song.Id
                    FROM [{_schemaName.Schema}].[{SongRelationShip.Song}] song
                    WHERE ISNULL(song.IsDeleted, 0) = 0
                      AND (@Keyword IS NULL OR song.Title LIKE '%' + @Keyword + '%')
                      AND (@CategoryId IS NULL OR EXISTS (
                          SELECT 1 FROM [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] filterMap
                          WHERE filterMap.SongId = song.Id AND filterMap.CategoryId = @CategoryId))
                    ORDER BY song.Id DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                )
                SELECT song.Id, song.Title, song.ArtistId, artist.Name AS ArtistName,
                       song.AlbumId, album.Title AS AlbumTitle, song.Duration, song.AudioUrl,
                       song.CoverImageUrl, song.Lyric, ISNULL(song.PlayCount, 0) AS PlayCount,
                       CAST(ISNULL(song.IsActive, 0) AS bit) AS IsActive,
                       category.Id AS CategoryId, category.Name AS CategoryName
                FROM PagedSongs page
                INNER JOIN [{_schemaName.Schema}].[{SongRelationShip.Song}] song ON song.Id = page.Id
                INNER JOIN [{_schemaName.Schema}].[{SongRelationShip.Artist}] artist ON artist.Id = song.ArtistId
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.Album}] album ON album.Id = song.AlbumId
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] map ON map.SongId = song.Id
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.SongCategory}] category
                    ON category.Id = map.CategoryId
                ORDER BY song.Id DESC, category.Name;
                """;

            using var multi = await connection.QueryMultipleAsync(new CommandDefinition(sql, new
            {
                Keyword = string.IsNullOrWhiteSpace(filter.Keyword) ? null : filter.Keyword.Trim(),
                filter.CategoryId,
                Offset = (filter.PageIndex - 1) * filter.PageSize,
                filter.PageSize
            }, cancellationToken: cancellationToken));

            var totalCount = await multi.ReadSingleAsync<int>();
            var rows = (await multi.ReadAsync<SongRow>()).ToList();
            return new PagedResult<SongDto>
            {
                Data = MapSongs(rows),
                TotalCount = totalCount,
                Page = filter.PageIndex,
                PageSize = filter.PageSize
            };
        });

    public Task<SongDto?> GetSongAsync(int id, CancellationToken cancellationToken = default)
        => QueryAsync(async connection =>
        {
            var rows = (await connection.QueryAsync<SongRow>(new CommandDefinition($"""
                SELECT song.Id, song.Title, song.ArtistId, artist.Name AS ArtistName,
                       song.AlbumId, album.Title AS AlbumTitle, song.Duration, song.AudioUrl,
                       song.CoverImageUrl, song.Lyric, ISNULL(song.PlayCount, 0) AS PlayCount,
                       CAST(ISNULL(song.IsActive, 0) AS bit) AS IsActive,
                       category.Id AS CategoryId, category.Name AS CategoryName
                FROM [{_schemaName.Schema}].[{SongRelationShip.Song}] song
                INNER JOIN [{_schemaName.Schema}].[{SongRelationShip.Artist}] artist ON artist.Id = song.ArtistId
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.Album}] album ON album.Id = song.AlbumId
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] map ON map.SongId = song.Id
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.SongCategory}] category
                    ON category.Id = map.CategoryId
                WHERE song.Id = @Id AND ISNULL(song.IsDeleted, 0) = 0
                ORDER BY category.Name
                """, new { Id = id }, cancellationToken: cancellationToken))).ToList();

            return MapSongs(rows).SingleOrDefault();
        });

    public Task<IReadOnlyList<SongCategoryDto>> GetSongCategoriesAsync(CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<SongCategoryDto>>(async connection =>
            (await connection.QueryAsync<SongCategoryDto>(new CommandDefinition($"""
                SELECT category.Id, category.Name, COUNT(song.Id) AS SongCount
                FROM [{_schemaName.Schema}].[{SongRelationShip.SongCategory}] category
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] map ON map.CategoryId = category.Id
                LEFT JOIN [{_schemaName.Schema}].[{SongRelationShip.Song}] song
                    ON song.Id = map.SongId AND ISNULL(song.IsDeleted, 0) = 0
                GROUP BY category.Id, category.Name
                ORDER BY category.Name
                """, cancellationToken: cancellationToken))).ToList());

    public Task<IReadOnlyList<SongCategoryMappingDto>> GetSongCategoryMappingsAsync(
        int? songId, int? categoryId, CancellationToken cancellationToken = default)
        => QueryAsync<IReadOnlyList<SongCategoryMappingDto>>(async connection =>
            (await connection.QueryAsync<SongCategoryMappingDto>(new CommandDefinition($"""
                SELECT map.Id, map.SongId, song.Title AS SongTitle,
                       map.CategoryId, category.Name AS CategoryName
                FROM [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] map
                INNER JOIN [{_schemaName.Schema}].[{SongRelationShip.Song}] song ON song.Id = map.SongId
                INNER JOIN [{_schemaName.Schema}].[{SongRelationShip.SongCategory}] category ON category.Id = map.CategoryId
                WHERE (@SongId IS NULL OR map.SongId = @SongId)
                  AND (@CategoryId IS NULL OR map.CategoryId = @CategoryId)
                  AND ISNULL(song.IsDeleted, 0) = 0
                ORDER BY map.Id DESC
                """, new { SongId = songId, CategoryId = categoryId }, cancellationToken: cancellationToken))).ToList());

    private Task<bool> ExistsAsync(string table, string predicate, object parameters, CancellationToken cancellationToken)
        => QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT CASE WHEN EXISTS (SELECT 1 FROM [{_schemaName.Schema}].[{table}] WHERE {predicate}) THEN 1 ELSE 0 END",
                parameters,
                cancellationToken: cancellationToken)) > 0);

    private static List<SongDto> MapSongs(IEnumerable<SongRow> rows)
        => rows.GroupBy(row => row.Id).Select(group =>
        {
            var row = group.First();
            return new SongDto
            {
                Id = row.Id,
                Title = row.Title,
                ArtistId = row.ArtistId,
                ArtistName = row.ArtistName,
                AlbumId = row.AlbumId,
                AlbumTitle = row.AlbumTitle,
                Duration = row.Duration,
                AudioUrl = row.AudioUrl,
                CoverImageUrl = row.CoverImageUrl,
                Lyric = row.Lyric,
                PlayCount = row.PlayCount,
                IsActive = row.IsActive,
                Categories = group.Where(item => item.CategoryId.HasValue)
                    .Select(item => new SongCategoryDto
                    {
                        Id = item.CategoryId!.Value,
                        Name = item.CategoryName ?? string.Empty
                    }).ToList()
            };
        }).ToList();

    private sealed class SongRow
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
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
