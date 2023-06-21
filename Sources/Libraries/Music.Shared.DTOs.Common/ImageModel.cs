using Music.Shared.Files.Common;

namespace Music.Shared.DTOs.Common;

public interface IImageBase
{
    string ImagePath { get; }
    string SmallImagePath { get; }
    string MediumImagePath { get; }
    string LargeImagePath { get; }
}

public abstract class ImageModelBase : IImageBase
{
    protected ImageModelBase(string imagePath)
    {
        ImagePath = imagePath;
    }

    public string ImagePath { get; }
    public string SmallImagePath => $"{ImagePath}/{ImageSizeEnum.Small}";
    public string MediumImagePath => $"{ImagePath}/{ImageSizeEnum.Medium}";
    public string LargeImagePath => $"{ImagePath}/{ImageSizeEnum.Large}";
}

public abstract record ImageRecordBase(string ImagePath) : IImageBase
{
    public string SmallImagePath => $"{ImagePath}/{ImageSizeEnum.Small}";
    public string MediumImagePath => $"{ImagePath}/{ImageSizeEnum.Medium}";
    public string LargeImagePath => $"{ImagePath}/{ImageSizeEnum.Large}";
}