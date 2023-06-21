using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc;
using Music.Image.Service.Models;
using Music.Image.Service.Models.Directories;
using Music.Services.Database.Common.Repositories;
using Music.Services.Exceptions;
using Music.Services.Exceptions.Exceptions;
using Music.Services.Files;
using Music.Services.Files.Providers;
using Music.Services.Identity.Common;
using Music.Shared.Files;
using Music.Shared.Files.Common;
using Music.Shared.Files.Helpers;
using Music.Shared.Services;
using static System.Drawing.Image;

namespace Music.Image.Service.Services;

public interface IImageService
{
    Task ChangeTrackImage(Guid trackId, IFormFile file, Guid changerId, HttpContext context);
    Task ChangePlaylistImage(Guid playlistId, IFormFile file, Guid changerId, HttpContext context);
    Task ChangeUserImage(Guid userId, IFormFile file, Guid changerId, HttpContext context);
    Task ChangeAlbumImage(Guid alumId, IFormFile file, Guid changerId, HttpContext context);
    Task ChangeArtistImage(Guid artistId, IFormFile file, Guid changerId, HttpContext context);
    Task<PhysicalFileResult> GetTrackImage(Guid trackId, ImageSizeEnum size);
    Task<PhysicalFileResult> GetPlaylistImage(Guid playlistId, ImageSizeEnum size);
    Task<PhysicalFileResult> GetUserImage(Guid userId, ImageSizeEnum size);
    Task<PhysicalFileResult> GetArtistImage(Guid artistId, ImageSizeEnum size);
    Task<PhysicalFileResult> GetAlbumImage(Guid albumId, ImageSizeEnum size);
}

public class ImageService : IImageService
{
    private readonly FileProviderSettings _fileProviderSettings;
    private readonly IRepository<Album> _albumsRepository;
    private readonly IRepository<Track> _tracksRepository;
    private readonly IRepository<Artist> _artistsRepository;
    private readonly IRepository<User> _usersRepository;
    private readonly IRepository<Playlist> _playlistsRepository;
    private readonly IRepository<AlbumFileDirectory> _albumFileDirectory;
    private readonly IRepository<ArtistFileDirectory> _artistFileDirectory;
    private readonly IRepository<TrackFileDirectory> _trackFileDirectory;
    private readonly IRepository<UserFileDirectory> _userFileDirectory;
    private readonly IRepository<PlaylistFileDirectory> _playlistFileDirectory;

    public ImageService(FileProviderSettings fileProviderSettings, IRepository<Album> albumsRepository,
        IRepository<Track> tracksRepository, IRepository<Artist> artistsRepository,
        IRepository<User> usersRepository, IRepository<Playlist> playlistsRepository,
        IRepository<AlbumFileDirectory> albumFileDirectory, IRepository<ArtistFileDirectory> artistFileDirectory,
        IRepository<TrackFileDirectory> trackFileDirectory, IRepository<UserFileDirectory> userFileDirectory,
        IRepository<PlaylistFileDirectory> playlistFileDirectory)
    {
        _fileProviderSettings = fileProviderSettings;
        _albumsRepository = albumsRepository;
        _tracksRepository = tracksRepository;
        _artistsRepository = artistsRepository;
        _usersRepository = usersRepository;
        _playlistsRepository = playlistsRepository;
        _albumFileDirectory = albumFileDirectory;
        _artistFileDirectory = artistFileDirectory;
        _trackFileDirectory = trackFileDirectory;
        _userFileDirectory = userFileDirectory;
        _playlistFileDirectory = playlistFileDirectory;
    }

    private string GetRootPath(Guid modelId, string serviceName) => 
        $"{_fileProviderSettings.RootPath}/{ServiceNames.Images}/{serviceName}/{modelId}";

    private static FileInfoModel CreateFileInfo(ImageSizeEnum imageSize) =>
        new()
        {
            DataType = FileTypeEnum.Image.GetDataType(),
            Extension = ".jpg",
            Name = imageSize.ToString()
        };

    private static List<FileInfoModel> CreateFileInfos() =>
        new()
        {
            CreateFileInfo(ImageSizeEnum.Small),
            CreateFileInfo(ImageSizeEnum.Medium),
            CreateFileInfo(ImageSizeEnum.Large),
        };

    private static (int Width, int Height) GetSize(ImageSizeEnum imageSizeEnum) => imageSizeEnum switch
    {
        ImageSizeEnum.Large => (512, 512),
        ImageSizeEnum.Medium => (256, 256),
        ImageSizeEnum.Small => (64, 64),
        _ => GetSize(ImageSizeEnum.Medium)
    };

    private static readonly ImageSizeEnum[] ImageSizes = { ImageSizeEnum.Small, ImageSizeEnum.Medium, ImageSizeEnum.Large };
    
    [SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
    private static List<byte[]> GetResizedImages(IFormFile file)
    {
        var result = new List<byte[]>();
        
        foreach (var imageSizeEnum in ImageSizes)
        {
            var image = FromStream(file.OpenReadStream());
            var size = GetSize(imageSizeEnum);
            var resized = new Bitmap(image, new Size(size.Width, size.Height));
            using var imageStream = new MemoryStream();
            resized.Save(imageStream, ImageFormat.Jpeg);
            result.Add(imageStream.ToArray());
        }

        return result;
    }

    private static async Task WriteImages(string rootPath, IFormFile file)
    {
        var resizedImages = GetResizedImages(file);
        var provider = new FileProvider(rootPath);
        await provider.WriteFile($"{nameof(ImageSizeEnum.Small)}.jpg", resizedImages[0]);
        await provider.WriteFile($"{nameof(ImageSizeEnum.Medium)}.jpg", resizedImages[1]);
        await provider.WriteFile($"{nameof(ImageSizeEnum.Large)}.jpg", resizedImages[2]);
    }

    public async Task ChangeTrackImage(Guid trackId, IFormFile file, Guid changerId, HttpContext context)
    {
        var track = await _tracksRepository.GetAsync(trackId)
                    ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        if (!context.IsAdmin() && track.OwnerId != changerId) 
            throw new ForbiddenException("Have no rights to change track image");
        
        CheckFile(file);

        var rootPath = GetRootPath(trackId, ServiceNames.Tracks);

        await WriteImages(rootPath, file);
        
        var fileInfo = await _trackFileDirectory.GetAsync(trackId);
        
        if (fileInfo == null)
        {
            fileInfo = new TrackFileDirectory()
            {
                Id = trackId,
                RootPath = rootPath,
                Files = CreateFileInfos()
            };
            
            await _trackFileDirectory.CreateAsync(fileInfo);
        }
        else
        {
            foreach (var fileInfoModel in fileInfo.Files)
            {
                fileInfoModel.ChangingDate = DateTimeOffset.UtcNow;
            }
            await _trackFileDirectory.UpdateAsync(fileInfo);
        }
    }
    
    public async Task ChangePlaylistImage(Guid playlistId, IFormFile file, Guid changerId, HttpContext context)
    {
        var playlist = await _playlistsRepository.GetAsync(playlistId)
                    ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);

        if (!context.IsAdmin() && playlist.OwnerId != changerId) 
            throw new ForbiddenException("Have no rights to change playlist image");
        
        CheckFile(file);

        var rootPath = GetRootPath(playlistId, ServiceNames.Playlists);

        await WriteImages(rootPath, file);
        
        var fileInfo = await _playlistFileDirectory.GetAsync(playlistId);
        
        if (fileInfo == null)
        {
            fileInfo = new PlaylistFileDirectory()
            {
                Id = playlistId,
                RootPath = rootPath,
                Files = CreateFileInfos()
            };
            
            await _playlistFileDirectory.CreateAsync(fileInfo);
        }
        else
        {
            foreach (var fileInfoModel in fileInfo.Files)
            {
                fileInfoModel.ChangingDate = DateTimeOffset.UtcNow;
            }
            await _playlistFileDirectory.UpdateAsync(fileInfo);
        }
    }
    public async Task ChangeUserImage(Guid userId, IFormFile file, Guid changerId, HttpContext context)
    {
        _ = await _usersRepository.GetAsync(userId)
            ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        if (!context.IsAdmin() && userId != changerId) 
            throw new ForbiddenException("Have no rights to change user image");
        
        CheckFile(file);

        var rootPath = GetRootPath(userId, ServiceNames.Users);

        await WriteImages(rootPath, file);
        
        var fileInfo = await _userFileDirectory.GetAsync(userId);
        
        if (fileInfo == null)
        {
            fileInfo = new UserFileDirectory()
            {
                Id = userId,
                RootPath = rootPath,
                Files = CreateFileInfos()
            };
            
            await _userFileDirectory.CreateAsync(fileInfo);
        }
        else
        {
            foreach (var fileInfoModel in fileInfo.Files)
            {
                fileInfoModel.ChangingDate = DateTimeOffset.UtcNow;
            }
            await _userFileDirectory.UpdateAsync(fileInfo);
        }
    }
    public async Task ChangeAlbumImage(Guid albumId, IFormFile file, Guid changerId, HttpContext context)
    {
        var album = await _albumsRepository.GetAsync(albumId)
                       ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);

        if (!context.IsAdmin() && album.OwnerId != changerId) 
            throw new ForbiddenException("Have no rights to change album image");
        
        CheckFile(file);

        var rootPath = GetRootPath(albumId, ServiceNames.Albums);

        await WriteImages(rootPath, file);
        
        var fileInfo = await _albumFileDirectory.GetAsync(albumId);
        
        if (fileInfo == null)
        {
            fileInfo = new AlbumFileDirectory()
            {
                Id = albumId,
                RootPath = rootPath,
                Files = CreateFileInfos()
            };
            
            await _albumFileDirectory.CreateAsync(fileInfo);
        }
        else
        {
            foreach (var fileInfoModel in fileInfo.Files)
            {
                fileInfoModel.ChangingDate = DateTimeOffset.UtcNow;
            }
            await _albumFileDirectory.UpdateAsync(fileInfo);
        }
    }
    public async Task ChangeArtistImage(Guid artistId, IFormFile file, Guid changerId, HttpContext context)
    {
        _ = await _artistsRepository.GetAsync(artistId)
            ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);

        if (!context.IsAdmin() && artistId != changerId) 
            throw new ForbiddenException("Have no rights to change artist image");
        
        CheckFile(file);

        var rootPath = GetRootPath(artistId, ServiceNames.Artists);

        await WriteImages(rootPath, file);
        
        var fileInfo = await _artistFileDirectory.GetAsync(artistId);
        
        if (fileInfo == null)
        {
            fileInfo = new ArtistFileDirectory()
            {
                Id = artistId,
                RootPath = rootPath,
                Files = CreateFileInfos()
            };
            
            await _artistFileDirectory.CreateAsync(fileInfo);
        }
        else
        {
            foreach (var fileInfoModel in fileInfo.Files)
            {
                fileInfoModel.ChangingDate = DateTimeOffset.UtcNow;
            }
            await _artistFileDirectory.UpdateAsync(fileInfo);
        }
    }

    private static PhysicalFileResult ReadFile(FileDirectory fileDirectory, ImageSizeEnum size)
    {
        var provider = new FileProvider(fileDirectory.RootPath);
        var fileInfo = fileDirectory.Files.First(x => x.Name == size.ToString());
        return provider.ReadFile($"{fileInfo.Name}{fileInfo.Extension}", fileInfo.DataType);
    }
    
    public async Task<PhysicalFileResult> GetTrackImage(Guid trackId, ImageSizeEnum size)
    {
        _ = await _tracksRepository.GetAsync(trackId)
            ?? throw new NotFoundException(ExceptionMessages.TrackNotFound);

        var fileInfo = await _trackFileDirectory.GetAsync(trackId)
                            ?? throw new NotFoundException(ExceptionMessages.ImageNotFound);

        return ReadFile(fileInfo, size);
    }
    public async Task<PhysicalFileResult> GetPlaylistImage(Guid playlistId, ImageSizeEnum size)
    {
        _ = await _playlistsRepository.GetAsync(playlistId)
            ?? throw new NotFoundException(ExceptionMessages.PlaylistNotFound);

        var fileInfo = await _playlistFileDirectory.GetAsync(playlistId)
                            ?? throw new NotFoundException(ExceptionMessages.ImageNotFound);

        return ReadFile(fileInfo, size);
    }

    public async Task<PhysicalFileResult> GetUserImage(Guid userId, ImageSizeEnum size)
    {
        _ = await _usersRepository.GetAsync(userId)
            ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

        var fileInfo = await _userFileDirectory.GetAsync(userId)
                       ?? throw new NotFoundException(ExceptionMessages.ImageNotFound);

        return ReadFile(fileInfo, size);
    }

    public async Task<PhysicalFileResult> GetArtistImage(Guid artistId, ImageSizeEnum size)
    {
        _ = await _artistsRepository.GetAsync(artistId)
            ?? throw new NotFoundException(ExceptionMessages.ArtistNotFound);

        var fileInfo = await _artistFileDirectory.GetAsync(artistId)
                       ?? throw new NotFoundException(ExceptionMessages.ImageNotFound);

        return ReadFile(fileInfo, size);
    }

    public async Task<PhysicalFileResult> GetAlbumImage(Guid albumId, ImageSizeEnum size)
    {
        _ = await _albumsRepository.GetAsync(albumId)
            ?? throw new NotFoundException(ExceptionMessages.AlbumNotFound);

        var fileInfo = await _albumFileDirectory.GetAsync(albumId)
                       ?? throw new NotFoundException(ExceptionMessages.ImageNotFound);

        return ReadFile(fileInfo, size);
    }

    private void CheckFile(IFormFile file)
    {
        if(file.Length == 0)
            throw new BadRequestException("File is empty!");
        if(file.Length > _fileProviderSettings.MaxFileSize)
            throw new BadRequestException($"Maximum file size is {_fileProviderSettings.MaxFileSize} bytes!");
        if(!_fileProviderSettings.AllowedExtensions.Contains(Path.GetExtension(file.FileName)))
            throw new BadRequestException("Unsupported media!");
    }
}