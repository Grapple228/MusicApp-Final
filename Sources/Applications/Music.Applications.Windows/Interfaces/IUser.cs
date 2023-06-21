namespace Music.Applications.Windows.Interfaces;

public interface IUser : IImageModel, IOwnerable
{
    public string Username { get; set; }
}