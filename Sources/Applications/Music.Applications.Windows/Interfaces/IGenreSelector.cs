using System.Collections.ObjectModel;
using Music.Shared.DTOs.Genres;

namespace Music.Applications.Windows.Interfaces;

public interface IGenreSelector
{
    ObservableCollection<GenreDto> Genres { get; }
    ObservableCollection<GenreDto> NotSelectedGenres { get; }
}