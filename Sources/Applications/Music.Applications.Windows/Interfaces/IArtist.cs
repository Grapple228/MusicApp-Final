using Music.Shared.Common;

namespace Music.Applications.Windows.Interfaces;

public interface IArtist : IModel, IImageModel, IOwnerable, IPlayable
{
    public string Name { get; }
}