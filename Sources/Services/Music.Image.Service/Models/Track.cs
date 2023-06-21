namespace Music.Image.Service.Models;

public class Track : IModelWithOwner
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; set; }
}