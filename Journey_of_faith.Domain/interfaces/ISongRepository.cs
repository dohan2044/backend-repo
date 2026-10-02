using System.Data.Common;
using Journey_of_faith.Domain.entities.musics;

namespace Journey_of_faith.Domain.interfaces;

public interface ISongRepository
{
    Task<int> CreateSongCategoryAsync(SongCategory songCategory, CancellationToken token);
    Task<bool> UpdateSongCategoryAsync(int id, SongCategory songCategory, Guid userId, CancellationToken token);
    Task<bool> DeleteSongCategoryAsync(int id, Guid userId, CancellationToken token);


    Task<int> CreateArtistAsync(Artist artist, CancellationToken cancellationToken);
    Task<int> UpdateArtistAsync(int id, Artist artist, CancellationToken cancellationToken);
    Task<bool> DeleteArtistAsync(int id,Guid userid, CancellationToken cancellationToken);

    Task<int> CreateAlbumAsync(Album album, CancellationToken cancellationToken);
    Task<bool> DeleteAlbumAsync(int id, Guid userId, CancellationToken cancellationToken);


    Task<int> CreateSongAsync(Song song,int categoryId, CancellationToken cancellationToken);
    Task<bool> UpdateSongAsync(int id, Song song, Guid userId, CancellationToken cancellationToken);
    Task<bool> DeleteSongAsync(int id, Guid userId, CancellationToken cancellationToken);
    Task<int> CreateSongCategoryMappingAsync(SongCategoryMapping mapping, CancellationToken cancellationToken);
    Task<bool> DeleteSongCategoryMappingAsync(int songId, int categoryId, CancellationToken cancellationToken);


    Task<int> CreatePlaylistSongAsync(PlaylistSong playlistSong, CancellationToken cancellationToken);
    Task<int> CreatePlaylistAsync(Playlist playlist, CancellationToken cancellationToken);
    Task<int> CreateListeningHistoryAsync(ListeningHistory listeningHistory, CancellationToken cancellationToken);
    Task<int> CreateUserFavoriteSongAsync(UserFavoriteSong userFavoriteSong, CancellationToken cancellationToken);
}
