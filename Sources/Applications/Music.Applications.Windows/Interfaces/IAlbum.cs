using Music.Shared.Common;

namespace Music.Applications.Windows.Interfaces;

public interface IAlbum : IImageModel, IOwnerable, IPlayable
{
    public string Title { get; }
    public Guid OwnerId { get; }
    public DateOnly PublicationDate { get; }
}