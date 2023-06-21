using Music.Shared.Common;

namespace Music.Services.Models;

public interface IGenreBase : IModel
{
    string Value { get; set; }
    string Color { get; set; }
}

public class GenreBase : ModelBase, IGenreBase
{
    public string Value { get; set; } = null!;
    public string Color { get; set; } = null!;
}