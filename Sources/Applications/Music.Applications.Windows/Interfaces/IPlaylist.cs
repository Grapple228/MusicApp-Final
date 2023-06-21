using Music.Shared.Common;

namespace Music.Applications.Windows.Interfaces;

public interface IPlaylist : IModel, IImageModel, IOwnerable, IPlayable
{
    public string Title { get; }
}