using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Image.Service.Services;
using Music.Shared.Files.Common;

namespace Music.Image.Service.Controllers;


[ApiController]
[Route("api/[Controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }
    
    [Authorize]
    [HttpPost("Artists/{artistId:guid}")]
    public async Task<ActionResult> ChangeArtist(Guid artistId, IFormFile file)
    {
        await _imageService.ChangeArtistImage(artistId, file, GetUserId(), HttpContext);
        return NoContent();
    }
    [Authorize]
    [HttpPost("Tracks/{trackId:guid}")]
    public async Task<ActionResult> ChangeTrack(Guid trackId, IFormFile file)
    {
        await _imageService.ChangeTrackImage(trackId, file, GetUserId(), HttpContext);
        return NoContent();
    }
    [Authorize]
    [HttpPost("Albums/{albumId:guid}")]
    public async Task<ActionResult> ChangeAlbum(Guid albumId, IFormFile file)
    {
        await _imageService.ChangeAlbumImage(albumId, file, GetUserId(), HttpContext);
        return NoContent();
    }
    [Authorize]
    [HttpPost("Users/{userId:guid}")]
    public async Task<ActionResult> ChangeUser(Guid userId, IFormFile file)
    {
        await _imageService.ChangeUserImage(userId, file, GetUserId(), HttpContext);
        return NoContent();
    }
    [Authorize]
    [HttpPost("Playlists/{playlistId:guid}")]
    public async Task<ActionResult> ChangePlaylist(Guid playlistId, IFormFile file)
    {
        await _imageService.ChangePlaylistImage(playlistId, file, GetUserId(), HttpContext);
        return NoContent();
    }
    
    [HttpGet("Artists/{artistId:guid}/{size}")]
    public async Task<FileResult> GetArtistImage(Guid artistId, ImageSizeEnum size)
    {
        return await _imageService.GetArtistImage(artistId, size);
    }
    [HttpGet("Tracks/{trackId:guid}/{size}")]
    public async Task<FileResult> GetTrackImage(Guid trackId, ImageSizeEnum size)
    {
        return await _imageService.GetTrackImage(trackId, size);
    }
    [HttpGet("Albums/{albumId:guid}/{size}")]
    public async Task<FileResult> GetAlbumImage(Guid albumId, ImageSizeEnum size)
    {
        return await _imageService.GetAlbumImage(albumId, size);
    }
    [HttpGet("Users/{userId:guid}/{size}")]
    public async Task<FileResult> GetUserImage(Guid userId, ImageSizeEnum size)
    {
        return await _imageService.GetUserImage(userId, size);
    }
    [HttpGet("Playlists/{playlistId:guid}/{size}")]
    public async Task<FileResult> GetPlaylistImage(Guid playlistId, ImageSizeEnum size)
    {
        return await _imageService.GetPlaylistImage(playlistId, size);
    }
}