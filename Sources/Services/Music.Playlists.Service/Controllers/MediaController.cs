using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Playlists.Service.Services;
using Music.Services.DTOs.Extensions;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Media;

namespace Music.Playlists.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/Playlists/[Controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaServiceBase<MediaPlaylistDto> _mediaService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IPlaylistsService _playlistsService;
    private readonly IMediaServiceBase<MediaTrackDto> _trackMediaService;

    public MediaController(IMediaServiceBase<MediaPlaylistDto> mediaService, IPublishEndpoint publishEndpoint,
        IPlaylistsService playlistsService, IMediaServiceBase<MediaTrackDto> trackMediaService)
    {
        _mediaService = mediaService;
        _publishEndpoint = publishEndpoint;
        _playlistsService = playlistsService;
        _trackMediaService = trackMediaService;
    }

    [HttpGet("{playlistId:guid}")]
    public async Task<ActionResult<MediaPlaylistDto>> Get(Guid playlistId)
    {
        return await _mediaService.Get(playlistId, GetUserId());
    }
    
    [HttpGet("{playlistId:guid}/Tracks")]
    public async Task<IEnumerable<MediaTrackDto>> GetTracks(Guid playlistId)
    {
        var playlist = await _playlistsService.Get(playlistId);
        var ids = playlist.Tracks.Select(x => x.Id);
        return await _trackMediaService.GetAll(ids, GetUserId());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaPlaylistDto>>> GetAll([FromQuery]string? searchQuery)
    {
        var media = await _mediaService.GetAll(GetUserId());
        return Ok(searchQuery == null ? media : media.Where(x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower())));
    }
    
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAllUser()
    {
        return Ok(await _mediaService.GetAll(GetUserId(), isUser: true));
    }

    [HttpGet("Liked")]
    public async Task<ActionResult<IEnumerable<MediaPlaylistDto>>> GetLiked()
    {
        return Ok(await _mediaService.GetAllLiked(GetUserId()));
    }
    
    [HttpGet("Blocked")]
    public async Task<ActionResult<IEnumerable<MediaPlaylistDto>>> GetBlocked()
    {
        return Ok(await _mediaService.GetAllBlocked(GetUserId()));
    }
    
    [HttpGet("{pageNumber:long}-{countPerPage:long}/user")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAllPageUser([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _mediaService.GetPage(GetUserId(), pageNumber, countPerPage, isUser: true));
    }
    
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAllPage([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _mediaService.GetPage(GetUserId(), pageNumber, countPerPage));
    }

    [HttpGet("Liked/{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetLikedPage([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _mediaService.GetPageLiked(GetUserId(), pageNumber, countPerPage));
    }
    
    [HttpGet("Blocked/{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetBlockedPage([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _mediaService.GetPageBlocked(GetUserId(), pageNumber, countPerPage));
    }

    [HttpPost("{playlistId:guid}")]
    public async Task<IActionResult> ChangeMedia(Guid playlistId, MediaCreateRequest request)
    {
        var userId = GetUserId();
        await _mediaService.ChangeMedia(playlistId, request, userId);
        await _publishEndpoint.PublishPlaylistMediaChanged(playlistId, userId, request.IsLiked, request.IsBlocked);
        return NoContent();
    }
}