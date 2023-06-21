namespace Music.Image.Service.Models;

public class Playlist : IModelWithOwner
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; set; }
}