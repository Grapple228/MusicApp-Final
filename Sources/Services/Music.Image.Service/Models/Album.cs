namespace Music.Image.Service.Models;

public class Album : IModelWithOwner
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; set; }
}