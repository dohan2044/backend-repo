using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.entities.musics;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.common;
using Microsoft.Extensions.Options;

namespace Journey_of_faith.Infrastructure.repositories;

public class SongRepository : BaseRepository, ISongRepository
{
    public SongRepository(IDbConnectionFactory dbConnection, IOptions<TableSchemaName> options)
    : base(dbConnection, options)
    {
        
    }
    public async Task<int> CreateSongCategoryAsync(SongCategory songCategory, CancellationToken cancellationToken)
    {
        return await QueryAsync<int>(async connection =>
        {
           var sql = $@"insert into
            [{_schemaName.Schema}].[{SongRelationShip.SongCategory}] (Name)
            output inserted.Id values (@Name)";

            return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                sql, new { Name = songCategory.Name }, cancellationToken: cancellationToken));
        });
    }
    public async Task<bool> UpdateSongCategoryAsync(int id, SongCategory songCategory, Guid userId, CancellationToken cancellationToken)
    {
        return await QueryAsync(async connection =>
        {
            var affected = await connection.ExecuteAsync(new CommandDefinition($@"
                update [{_schemaName.Schema}].[{SongRelationShip.SongCategory}]
                set Name = @Name
                where Id = @Id",
                new { Id = id, Name = songCategory.Name },
                cancellationToken: cancellationToken));
            return affected > 0;
        });
    }
    public async Task<bool> DeleteSongCategoryAsync(int id, Guid userId, CancellationToken cancellationToken)
    {
        return await QueryAsync<bool>(async connection =>
        {
           var songCategory = await connection.ExecuteAsync(new CommandDefinition($@"
              delete from [{_schemaName.Schema}].[{SongRelationShip.SongCategory}]
              where Id = @Id", new { Id = id }, cancellationToken: cancellationToken));

           return songCategory > 0;
        });
    }
    
    public async Task<int> CreateArtistAsync(Artist artist, CancellationToken cancellationToken)
    {
        return await QueryAsync<int>(async connection =>
        {
           var sql = $@"insert into [{_schemaName.Schema}].[{SongRelationShip.Artist}] (Name, Description, ImageUrl) values
            values(@Name, @Description, @ImageUrl)
           " ;
           return await connection.ExecuteAsync(sql: sql, new { Name = artist.Name, Description = artist.Description, ImageUrl = artist.ImageUrl});
        });
    }
    public async Task<int> UpdateArtistAsync(int id, Artist artist, CancellationToken cancellationToken)
    {
        return await QueryAsync<int>(async connection =>
        {
           return await connection.ExecuteAsync("sp_UpdateArtist", new
           {
               Id = id,
               Name = artist.Name,
               Description = artist.Description,
               ImageUrl = artist.ImageUrl,
           },commandType: System.Data.CommandType.StoredProcedure); 
        });
    }

    public async Task<bool> DeleteArtistAsync(int id, Guid userId, CancellationToken cancellationToken)
    {
        return await QueryAsync<bool>(async connection =>
        {
            var artist = await connection.ExecuteAsync($@"
              update from [{_schemaName.Schema}].[{SongRelationShip.Artist}]
                set IsDeleted = true,
                    DeletionTime = getdate(),
                    LastModificationTime = getdate(),
                    LastModifierUserId = @userId,
                    DeleterUserId = @userId
                where Id = @Id
           ", new
           {
                userId = userId,
                Id = id                                                  
           });

           return artist > 0;
        });
    }

    public async Task<int> CreateAlbumAsync(Album album, CancellationToken cancellationToken)
    {
        return await QueryAsync<int>(async connection =>
        {
            var sql = $@"insert into [{_schemaName.Schema}].[{SongRelationShip.Album}] (Title, ArtistId, ReleaseYear, CoverImageUrl)
            values(@Title, @ArtistId, @ReleaseYear, @CoverImageUrl)";

            return await connection.ExecuteAsync(sql: sql, new
            {
                Title = album.Title,
                ArtistId = album.ArtistId,
                ReleaseYear = album.ReleaseYear,
                CoverImageUrl = album.CoverImageUrl,
            });
        });
    }

    public async Task<bool> DeleteAlbumAsync(int id, Guid userId, CancellationToken cancellationToken)
    {
        return await QueryAsync<bool>(async connection =>
        {
           var artist = await connection.ExecuteAsync($@"
              update from [{_schemaName.Schema}].[{SongRelationShip.Album}]
                set IsDeleted = true,
                    DeletionTime = getdate(),
                    LastModificationTime = getdate(),
                    LastModifierUserId = @userId,
                    DeleterUserId = @userId
                where Id = @Id
           ", new
           {
                userId = userId,
                Id = id                                                  
           });

           return artist > 0;
        });
    }

    public async Task<int> CreateSongAsync(Song song,int categoryId, CancellationToken token)
    {
        return await QueryAsync<int>(async connection =>
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var songId = await connection.ExecuteScalarAsync<int>(new CommandDefinition($@"
                    insert into [{_schemaName.Schema}].[{SongRelationShip.Song}]
                        (Title, ArtistId, AlbumId, Duration, AudioUrl, CoverImageUrl, Lyric, PlayCount, IsActive)
                    output inserted.Id
                    values (@Title, @ArtistId, @AlbumId, @Duration, @AudioUrl, @CoverImageUrl, @Lyric, @PlayCount, @IsActive)",
                    new
                    {
                        song.Title,
                        song.ArtistId,
                        song.AlbumId,
                        song.Duration,
                        song.AudioUrl,
                        song.CoverImageUrl,
                        song.Lyric,
                        song.PlayCount,
                        song.IsActive
                    }, transaction, cancellationToken: token));

                await connection.ExecuteAsync(new CommandDefinition($@"
                insert into [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] (SongId, CategoryId)
                values(@SongId, @CategoryId)
                ", new { SongId = songId, CategoryId = categoryId }, transaction, cancellationToken: token));

                transaction.Commit();
                return songId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        });
    }

    public async Task<bool> UpdateSongAsync(int id, Song song, Guid userId, CancellationToken cancellationToken)
    {
        return await QueryAsync(async connection =>
        {
            var affected = await connection.ExecuteAsync(new CommandDefinition($@"
                update [{_schemaName.Schema}].[{SongRelationShip.Song}]
                set Title = @Title, ArtistId = @ArtistId, AlbumId = @AlbumId,
                    Duration = @Duration, AudioUrl = @AudioUrl, CoverImageUrl = @CoverImageUrl,
                    Lyric = @Lyric, PlayCount = @PlayCount, IsActive = @IsActive,
                    LastModificationTime = getutcdate(), LastModifierUserId = @UserId
                where Id = @Id and ISNULL(IsDeleted, 0) = 0",
                new
                {
                    Id = id,
                    song.Title,
                    song.ArtistId,
                    song.AlbumId,
                    song.Duration,
                    song.AudioUrl,
                    song.CoverImageUrl,
                    song.Lyric,
                    song.PlayCount,
                    song.IsActive,
                    UserId = userId
                }, cancellationToken: cancellationToken));
            return affected > 0;
        });
    }
    
    public async Task<bool> DeleteSongAsync(int id, Guid userId, CancellationToken cancellationToken)
    {
        return await QueryAsync<bool>(async connection =>
        {
           var song = await connection.ExecuteAsync($@"
              update [{_schemaName.Schema}].[{SongRelationShip.Song}]
                set IsDeleted = 1,
                    DeletionTime = getutcdate(),
                    LastModificationTime = getutcdate(),
                    LastModifierUserId = @userId,
                    DeleterUserId = @userId
                where Id = @Id and ISNULL(IsDeleted, 0) = 0
           ", new
           {
                userId = userId,
                Id = id                                                  
           });

           return song > 0;
        });
    }

    public async Task<int> CreateSongCategoryMappingAsync(SongCategoryMapping mapping, CancellationToken cancellationToken)
    {
        return await QueryAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition($@"
                insert into [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}] (SongId, CategoryId)
                output inserted.Id values (@SongId, @CategoryId)",
                new { mapping.SongId, mapping.CategoryId }, cancellationToken: cancellationToken)));
    }

    public async Task<bool> DeleteSongCategoryMappingAsync(int songId, int categoryId, CancellationToken cancellationToken)
    {
        return await QueryAsync(async connection =>
            await connection.ExecuteAsync(new CommandDefinition($@"
                delete from [{_schemaName.Schema}].[{SongRelationShip.SongCategoryMapping}]
                where SongId = @SongId and CategoryId = @CategoryId",
                new { SongId = songId, CategoryId = categoryId }, cancellationToken: cancellationToken)) > 0);
    }
    public async Task<int> CreatePlaylistSongAsync(PlaylistSong playlistSong, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public async Task<int> CreatePlaylistAsync(Playlist playlist, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateListeningHistoryAsync(ListeningHistory history, CancellationToken cancellationToken)
    {
        return await QueryAsync<int>(async connection =>
        {
           return await connection.ExecuteAsync($@"insert into [{_schemaName.Schema}].[{SongRelationShip.ListeningHistory}] (UserId, SongId, ListenTime)
                values(@UserId, @SongId, @ListenTime
           ", new
           {
                UserId = history.UserId,
                SongId = history.SongId,
                ListenTime = DateTime.UtcNow
           });
        });
    }

    public async Task<int> CreateUserFavoriteSongAsync(UserFavoriteSong userFavoriteSong,CancellationToken cancellationToken)
    {
        return await QueryAsync<int>(async connection =>
        {
            return await connection.ExecuteAsync($@"insert into [{_schemaName.Schema}].[{SongRelationShip.UserFavoriteSong}] (UserId, SongId, CreatedTime)
                values(@UserId, @SongId, @CreatedTime)
            ", new {UserId = userFavoriteSong.UserId, SongId = userFavoriteSong.SongId, CreatedTime = DateTime.UtcNow});
        });
    }
}


public static class SongRelationShip
{
    public const string UserFavoriteSong = "UserFavoriteSong";
    public const string ListeningHistory = "ListeningHistory";
    public const string Playlist = "Playlist";
    public const string Song = "Song";
    public const string PlaylistSong = "PlaylistSong";
    public const string SongCategoryMapping = "SongCategoryMapping";
    public const string SongCategory = "SongCategory";
    public const string Artist = "Artist";
    public const string Album = "Album";
}
