namespace Music.Services.MassTransit.Contracts;

public interface IMediaChanged
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public bool IsLiked { get; }
    public bool IsBlocked { get; }
}