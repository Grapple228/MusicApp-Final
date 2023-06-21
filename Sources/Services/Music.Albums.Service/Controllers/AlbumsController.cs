using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Albums.Service.Services;
using Music.Services.Identity.Common;
using Music.Shared.Common;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;
using Music.Shared.DTOs.Requests.Albums;

namespace Music.Albums.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumsService _albumsService;
    
    public AlbumsController(IAlbumsService albumsService)
    {
        _albumsService = albumsService;
    }

    //GET /api/albums/pageNumber-countPerPage
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<PageResult<AlbumDto>>> GetPageAsync([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, 1000)] long countPerPage = 30, string? searchQuery = null)
    {
        return Ok(await _albumsService.GetPage(pageNumber, countPerPage, searchQuery));
    }

    //GET /api/albums
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAll([FromQuery]string? searchQuery, [FromQuery][Range(1, 1000)]long count = 30)
    {
        var media = await _albumsService.GetAll(searchQuery: searchQuery, count: count);
        return Ok(media);
    }
    
    //GET /api/albums/my
    [HttpGet("my")]
    [Authorize(Policy = Policies.ArtistPolicy)]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> GetUserAsync()
    {
        return Ok(await _albumsService.GetUserAlbums(GetUserId()));
    }

    //GET /api/albums/{albumId}
    [HttpGet("{albumId:guid}")]
    public async Task<ActionResult<AlbumDto>> GetByIdAsync(Guid albumId)
    {
        return await _albumsService.Get(albumId);
    }

    //POST /api/albums
    [HttpPost]
    [Authorize(Policy = Policies.ArtistPolicy)]
    public async Task<ActionResult<AlbumDto>> PostAsync(AlbumCreateRequest request)
    {
        var album = await _albumsService.Create(GetUserId(), request, HttpContext);
        var actionName = nameof(GetByIdAsync);
        return CreatedAtAction(actionName, new { albumId = album.Id }, album);
    }
    
    //POST /api/albums
    [HttpPost("{artistId:guid}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<ActionResult<AlbumDto>> PostAsync(Guid artistId, AlbumCreateRequest request)
    {
        var album = await _albumsService.Create(artistId, request, HttpContext);
        var actionName = nameof(GetByIdAsync);
        return CreatedAtAction(actionName, new { albumId = album.Id }, album);
    }

    //PUT /api/albums/{albumId}/title
    [HttpPut("{albumId:guid}/ChangeTitle/{title}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> ChangeTitleAsync(Guid albumId, [StringValidation(ErrorMessage = DtoErrorMessages.InvalidTitle)] string title)
    {
        await _albumsService.ChangeAlbumTitle(albumId, title, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //PUT /api/albums/{albumId}/isPublic
    [HttpPut("{albumId:guid}/{isPublic:bool}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> ChangePublicationDateAsync(Guid albumId, bool isPublic)
    {
        await _albumsService.ChangeAlbumPrivacy(albumId, isPublic, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //PUT /api/albums/{albumId}/publicationDate
    [HttpPut("{albumId:guid}/{publicationDate}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> ChangePrivacyAsync(Guid albumId, DateOnly publicationDate)
    {
        await _albumsService.ChangeAlbumPublicationDate(albumId, publicationDate, GetUserId(), HttpContext);
        return NoContent();
    }

    //DELETE /api/albums/{albumId}
    [HttpDelete("{albumId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> DeleteAsync(Guid albumId)
    {
        await _albumsService.Delete(albumId, GetUserId(), HttpContext);
        return NoContent();
    }

    [HttpPost("{albumId:guid}/Tracks/{trackId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> AddTrackToAlbum(Guid albumId, Guid trackId)
    {
        await _albumsService.AddTrackToAlbum(albumId, trackId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    [HttpDelete("{albumId:guid}/Tracks/{trackId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> RemoveTrackFromAlbum(Guid albumId, Guid trackId)
    {
        await _albumsService.RemoveTrackFromAlbum(albumId, trackId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    [HttpPost("{albumId:guid}/Artists/{artistId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> AddArtistToAlbum(Guid albumId, Guid artistId)
    {
        await _albumsService.AddArtistToAlbum(albumId, artistId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    [HttpDelete("{albumId:guid}/Artists/{artistId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> RemoveArtistFromAlbum(Guid albumId, Guid artistId)
    {
        await _albumsService.RemoveArtistFromAlbum(albumId, artistId, GetUserId(), HttpContext);
        return NoContent();
    }
}