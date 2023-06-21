using Music.Shared.Common;

namespace Music.Applications.Windows.Interfaces;

public interface IMediaModel : IModel
{
    bool IsLiked { get; set; }
    bool IsBlocked { get; set; }
}