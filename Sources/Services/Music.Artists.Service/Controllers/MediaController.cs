using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Artists.Service.Services;
using Music.Services.DTOs.Extensions;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Media;

namespace Music.Artists.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/Artists/[Controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaServiceBase<MediaArtistDto> _mediaService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMediaServiceBase<MediaTrackDto> _trackMediaService;
    private readonly IArtistsService _artistsService;

    public MediaController(IMediaServiceBase<MediaArtistDto> mediaService, IPublishEndpoint publishEndpoint, IMediaServiceBase<MediaTrackDto> trackMediaService,
        IArtistsService artistsService)
    {
        _mediaService = mediaService;
        _publishEndpoint = publishEndpoint;
        _trackMediaService = trackMediaService;
        _artistsService = artistsService;
    }

    [HttpGet("{artistId:guid}")]
    public async Task<ActionResult<MediaArtistDto>> Get(Guid artistId)
    {
        return await _mediaService.Get(artistId, GetUserId());
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaArtistDto>>> GetAll([FromQuery]string? searchQuery)
    {
        var media = await _mediaService.GetAll(GetUserId());
        return Ok(searchQuery == null ? media : media.Where(x => x.Name.ToLower().Contains(searchQuery.Trim().ToLower())));
    }
    
    [HttpGet("{artistId:guid}/Tracks")]
    public async Task<IEnumerable<MediaTrackDto>> GetTracks(Guid artistId)
    {
        var artist = await _artistsService.Get(artistId);
        var ids = artist.Tracks.Select(x => x.Id);
        return await _trackMediaService.GetAll(ids, GetUserId());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaArtistDto>>> GetAll()
    {
        return Ok(await _mediaService.GetAll(GetUserId()));
    }
    
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAllUser()
    {
        return Ok(await _mediaService.GetAll(GetUserId(), isUser: true));
    }

    [HttpGet("Liked")]
    public async Task<ActionResult<IEnumerable<MediaArtistDto>>> GetLiked()
    {
        return Ok(await _mediaService.GetAllLiked(GetUserId()));
    }
    
    [HttpGet("Blocked")]
    public async Task<ActionResult<IEnumerable<MediaArtistDto>>> GetBlocked()
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

    [HttpPost("{artistId:guid}")]
    public async Task<IActionResult> ChangeMedia(Guid artistId, MediaCreateRequest request)
    {
        var userId = GetUserId();
        await _mediaService.ChangeMedia(artistId, request, userId);
        await _publishEndpoint.PublishArtistMediaChanged(artistId, userId, request.IsLiked, request.IsBlocked);
        return NoContent();
    }
}