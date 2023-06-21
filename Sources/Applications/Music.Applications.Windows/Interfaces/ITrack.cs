using System.Collections.ObjectModel;
using Music.Shared.DTOs.Genres;

namespace Music.Applications.Windows.Interfaces;

public interface ITrack : IImageModel, IOwnerable, IPlayable
{
    public string Title { get; }
    public Guid OwnerId { get; }
    public DateOnly PublicationDate { get; }
    public int Duration { get; }
    public string DurationString { get; }
    ObservableCollection<GenreDto> Genres { get; }
}