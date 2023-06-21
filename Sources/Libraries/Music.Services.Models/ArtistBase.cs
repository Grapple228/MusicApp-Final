using Music.Shared.Common;

namespace Music.Services.Models;

public interface IArtistBase : IModel
{
    string Name { get; set; }
}

public class ArtistBase : ModelBase, IArtistBase
{
    public string Name { get; set; } = null!;
}