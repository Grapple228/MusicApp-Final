using Music.Shared.Common;

namespace Music.Applications.Windows.Interfaces;

public interface IPlayable : IModel
{
    public bool IsPlaying { get; set; }
    public bool IsCurrent { get; set; }
}