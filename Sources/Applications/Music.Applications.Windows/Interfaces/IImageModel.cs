namespace Music.Applications.Windows.Interfaces;

public interface IImageModel
{
    string ImagePath { get; set; }
    string SmallImagePath { get; set; }
    string MediumImagePath { get; set; }
    string LargeImagePath { get; set; }
}