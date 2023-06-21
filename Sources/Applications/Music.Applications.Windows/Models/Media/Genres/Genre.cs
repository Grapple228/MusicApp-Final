using Music.Applications.Windows.Core;
using Music.Shared.Common;
using Music.Shared.DTOs.Genres;

namespace Music.Applications.Windows.Models.Media.Genres;

public class Genre : ObservableObject, IModel
{
    private string _color = null!;
    private string _value = null!;

    public Genre(GenreDto genreDto)
    {
        Id = genreDto.Id;
        Update(genreDto);
    }

    public string Value
    {
        get => _value;
        set
        {
            if (value == _value) return;
            _value = value;
            OnPropertyChanged();
        }
    }

    public string Color
    {
        get => _color;
        set
        {
            if (value == _color) return;
            _color = value;
            OnPropertyChanged();
        }
    }

    public Guid Id { get; init; }

    public void Update(GenreDto genreDto)
    {
        Color = genreDto.Color;
        Value = genreDto.Value;
    }
}