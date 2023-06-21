using Music.Shared.Common;

namespace Music.Services.Models;

public interface IAlbumBase : IModel
{
    string Title { get; set; }
    Guid OwnerId { get; set; }
    DateOnly PublicationDate { get; set; }
    bool IsPublic { get; set; }
}

public class AlbumBase : ModelBase, IAlbumBase
{
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateOnly PublicationDate { get; set; }
    public bool IsPublic { get; set; }
}