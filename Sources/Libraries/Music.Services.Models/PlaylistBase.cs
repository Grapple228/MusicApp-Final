using Music.Shared.Common;

namespace Music.Services.Models;

public interface IPlaylistBase : IModel
{
    string Title { get; set; }
    Guid OwnerId { get; set; }
    bool IsPublic { get; set; }
}

public class PlaylistBase : ModelBase, IPlaylistBase
{
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public bool IsPublic { get; set; }
}