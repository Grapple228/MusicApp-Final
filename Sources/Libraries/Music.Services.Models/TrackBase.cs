using Music.Shared.Common;

namespace Music.Services.Models;

public interface ITrackBase : IModel
{
    string Title { get; set; }
    Guid OwnerId { get; set; }

    /// <summary>
    /// Miliseconds
    /// </summary>
    int Duration { get; set; }

    DateOnly PublicationDate { get; set; }
    bool IsPublic { get; set; }
}

public class TrackBase : ModelBase, ITrackBase
{
    public string Title { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public int Duration { get; set; }
    public DateOnly PublicationDate { get; set; }
    public bool IsPublic { get; set; }
}