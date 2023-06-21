using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Services.DTOs.Extensions;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Media;

namespace Music.Tracks.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/Tracks/[Controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaServiceBase<MediaTrackDto> _mediaService;
    private readonly IPublishEndpoint _publishEndpoint;

    public MediaController(IMediaServiceBase<MediaTrackDto> mediaService, IPublishEndpoint publishEndpoint)
    {
        _mediaService = mediaService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet("{trackId:guid}")]
    public async Task<ActionResult<MediaTrackDto>> Get(Guid trackId)
    {
        return await _mediaService.Get(trackId, GetUserId());
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaTrackDto>>> GetAll([FromQuery]string? searchQuery)
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
    public async Task<ActionResult<IEnumerable<MediaTrackDto>>> GetLiked()
    {
        return Ok(await _mediaService.GetAllLiked(GetUserId()));
    }
    
    [HttpGet("Blocked")]
    public async Task<ActionResult<IEnumerable<MediaTrackDto>>> GetBlocked()
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

    [HttpPost("{trackId:guid}")]
    public async Task<IActionResult> ChangeMedia(Guid trackId, MediaCreateRequest request)
    {
        var userId = GetUserId();
        await _mediaService.ChangeMedia(trackId, request, userId);
        await _publishEndpoint.PublishTrackMediaChanged(trackId, userId, request.IsLiked, request.IsBlocked);
        return NoContent();
    }
}