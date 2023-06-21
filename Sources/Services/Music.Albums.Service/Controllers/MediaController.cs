using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Albums.Service.Models;
using Music.Albums.Service.Services;
using Music.Services.Database.Common.Repositories;
using Music.Services.DTOs.Extensions;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Requests.Media;

namespace Music.Albums.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/Albums/[Controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaServiceBase<MediaAlbumDto> _albumMediaService;
    private readonly IAlbumsService _albumsService;
    private readonly IMediaServiceBase<MediaTrackDto> _trackMediaService;
    private readonly IPublishEndpoint _publishEndpoint;

    public MediaController(IMediaServiceBase<MediaAlbumDto> albumMediaService, IAlbumsService albumsService, IMediaServiceBase<MediaTrackDto> trackMediaService, IPublishEndpoint publishEndpoint)
    {
        _albumMediaService = albumMediaService;
        _albumsService = albumsService;
        _trackMediaService = trackMediaService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet("{albumId:guid}")]
    public async Task<ActionResult<MediaAlbumDto>> Get(Guid albumId)
    {
        return await _albumMediaService.Get(albumId, GetUserId());
    }
    
    [HttpGet("{albumId:guid}/Tracks")]
    public async Task<IEnumerable<MediaTrackDto>> GetTracks(Guid albumId)
    {
        var album = await _albumsService.Get(albumId);
        var ids = album.Tracks.Select(x => x.Id);
        return await _trackMediaService.GetAll(ids, GetUserId());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAll([FromQuery]string? searchQuery)
    {
        var media = await _albumMediaService.GetAll(GetUserId());
        return Ok(searchQuery == null ? media : media.Where(x => x.Title.ToLower().Contains(searchQuery.Trim().ToLower())));
    }
    
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAllUser()
    {
        return Ok(await _albumMediaService.GetAll(GetUserId(), isUser: true));
    }

    [HttpGet("Liked")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetLiked()
    {
        return Ok(await _albumMediaService.GetAllLiked(GetUserId()));
    }
    
    [HttpGet("Blocked")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetBlocked()
    {
        return Ok(await _albumMediaService.GetAllBlocked(GetUserId()));
    }
    
    [HttpGet("{pageNumber:long}-{countPerPage:long}/user")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAllPageUser([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _albumMediaService.GetPage(GetUserId(), pageNumber, countPerPage, isUser: true));
    }
    
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetAllPage([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _albumMediaService.GetPage(GetUserId(), pageNumber, countPerPage));
    }

    [HttpGet("Liked/{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetLikedPage([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _albumMediaService.GetPageLiked(GetUserId(), pageNumber, countPerPage));
    }
    
    [HttpGet("Blocked/{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<IEnumerable<MediaAlbumDto>>> GetBlockedPage([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, long.MaxValue)] long countPerPage)
    {
        return Ok(await _albumMediaService.GetPageBlocked(GetUserId(), pageNumber, countPerPage));
    }

    [HttpPost("{albumId:guid}")]
    public async Task<IActionResult> ChangeMedia(Guid albumId, MediaCreateRequest request)
    {
        var userId = GetUserId();
        await _albumMediaService.ChangeMedia(albumId, request, userId);
        await _publishEndpoint.PublishAlbumMediaChanged(albumId, userId, request.IsLiked, request.IsBlocked);
        return NoContent();
    }
}