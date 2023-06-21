using Music.Shared.Common;

namespace Music.Services.Models.Media;

public interface IMediaModelBase : IMedia
{
    Guid UserId { get; init; }
    Guid MediaId { get; init; }
}

public class MediaModelBase : ModelBase, IMediaModelBase
{
    public bool IsLiked { get; set; }
    public bool IsBlocked { get; set; }
    public Guid UserId { get; init; }
    public Guid MediaId { get; init; }
}