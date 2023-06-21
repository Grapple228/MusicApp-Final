namespace Music.Shared.Common;

public interface IMedia : IModel
{
    public bool IsLiked { get; set; }
    public bool IsBlocked { get; set; }
}