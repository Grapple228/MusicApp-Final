using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Services.Identity.Common;
using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;
using Music.Shared.DTOs.Tracks;
using Music.Tracks.Service.Requests;
using Music.Tracks.Service.Services;

namespace Music.Tracks.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TracksController : ControllerBase
{
    private readonly ITracksService _tracksService;

    public TracksController(ITracksService tracksService)
    {
        _tracksService = tracksService;
    }
    
    //GET /api/tracks/pageNumber-countPerPage
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<PageResult<TrackDto>>> GetPageAsync([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, 1000)] long countPerPage = 30, string? searchQuery = null)
    {
        return Ok(await _tracksService.GetPage(pageNumber, countPerPage, searchQuery));
    }

    //GET /api/tracks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrackDto>>> GetAll([FromQuery]string? searchQuery, [FromQuery][Range(1, 1000)]long count = 30)
    {
        var media = await _tracksService.GetAll(searchQuery: searchQuery, count: count);
        return Ok(media);
    }
    
    //GET /api/tracks/my
    [HttpGet("my")]
    [Authorize(Policy = Policies.ArtistPolicy)]
    public async Task<ActionResult<IEnumerable<TrackDto>>> GetUserAsync()
    {
        return Ok(await _tracksService.GetUserTracks(GetUserId()));
    }

    //GET /api/tracks/{trackId}
    [HttpGet("{trackId:guid}")]
    public async Task<ActionResult<TrackDto>> GetByIdAsync(Guid trackId)
    {
        return await _tracksService.Get(trackId);
    }
    
    //POST /api/tracks
    [HttpPost]
    [Authorize(Policy = Policies.ArtistPolicy)]
    public async Task<ActionResult<TrackDto>> UploadTrack(CreateTrackRequest request)
    {
        return await _tracksService.Create(GetUserId(), request);
    }
    
    //POST /api/tracks
    [HttpPost("{artistId:guid}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<ActionResult<TrackDto>> UploadTrack(Guid artistId, CreateTrackRequest request)
    {
        return await _tracksService.Create(artistId, request);
    }
    
    [HttpPut("{trackId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<int> ChangeTrack(Guid trackId, IFormFile file)
    {
        return await _tracksService.ChangeTrackFile(trackId, GetUserId(), file, HttpContext);
    }

    //PUT /api/tracks/{trackId}/title
    [HttpPut("{trackId:guid}/ChangeTitle/{title}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> ChangeTitle(Guid trackId, [StringValidation(ErrorMessage = DtoErrorMessages.InvalidTitle)] string title)
    {
        await _tracksService.ChangeTrackTitle(trackId, title, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //PUT /api/tracks/{trackId}/publicationDate
    [HttpPut("{trackId:guid}/{publicationDate}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> ChangePublicationDate(Guid trackId, DateOnly publicationDate)
    {
        await _tracksService.ChangeTrackPublicationDate(trackId, publicationDate, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //PUT /api/tracks/{trackId}/isPublic
    [HttpPut("{trackId:guid}/{isPublic:bool}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> ChangePrivacy(Guid trackId, bool isPublic)
    {
        await _tracksService.ChangeTrackPrivacy(trackId, isPublic, GetUserId(), HttpContext);
        return NoContent();
    }

    //DELETE /api/tracks/{trackId}
    [HttpDelete("{trackId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> DeleteAsync(Guid trackId)
    {
        await _tracksService.Delete(trackId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //POST /api/tracks/{trackId}/Genres/{genreId}
    [HttpPost("{trackId:guid}/Genres/{genreId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> AddGenreToTrack(Guid trackId, Guid genreId)
    {
        await _tracksService.AddGenreToTrack(trackId, genreId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //DELETE /api/tracks/{trackId}/Genres/{genreId}
    [HttpDelete("{trackId:guid}/Genres/{genreId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> RemoveGenreFromTrack(Guid trackId, Guid genreId)
    {
        await _tracksService.RemoveGenreFromTrack(trackId, genreId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //POST /api/tracks/{trackId}/artists/{artistId}
    [HttpPost("{trackId:guid}/Artists/{artistId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> AddArtistToTrack(Guid trackId, Guid artistId)
    {
        await _tracksService.AddArtistToTrack(trackId, artistId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //DELETE /api/tracks/{trackId}/artists/{artistId}
    [HttpDelete("{trackId:guid}/Artists/{artistId:guid}")]
    [Authorize(Policy = Policies.ArtistOrAdminPolicy)]
    public async Task<IActionResult> RemoveArtistFromTrack(Guid trackId, Guid artistId)
    {
        await _tracksService.RemoveArtistFromTrack(trackId, artistId, GetUserId(), HttpContext);
        return NoContent();
    }
}