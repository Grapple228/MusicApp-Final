using Music.Shared.Common;

namespace Music.Shared.DTOs.Playlists;

public record PlaylistShort(Guid Id, string Title, Guid OwnerId) : IModel;