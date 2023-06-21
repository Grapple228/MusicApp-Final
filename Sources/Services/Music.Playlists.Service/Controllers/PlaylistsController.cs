using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Playlists.Service.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Common;
using Music.Shared.DTOs.Common.Attributes;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Requests.Playlists;

namespace Music.Playlists.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistsService _playlistsService;

    public PlaylistsController(IPlaylistsService playlistsService)
    {
        _playlistsService = playlistsService;
    }

    //GET /api/playlists/my
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<PlaylistDto>>> GetUserAsync()
    {
        return Ok(await _playlistsService.GetUserPlaylists(GetUserId()));
    }
    
    //GET /api/playlists
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaylistDto>>> GetAll([FromQuery]string? searchQuery, [FromQuery][Range(1, 1000)]long count = 30)
    {
        var media = await _playlistsService.GetAll(searchQuery: searchQuery, count: count);
        return Ok(media);
    }

    //GET /api/playlists/{pageNumber}-{countPerPage}
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<PageResult<PlaylistDto>>> GetPageAsync([Range(1, long.MaxValue)] long pageNumber,
        [Range(1, 1000)] long countPerPage = 30, string? searchQuery = null)
    {
        return Ok(await _playlistsService.GetPage(pageNumber, countPerPage, searchQuery));
    }

    //GET /api/playlists/{playlistId}
    [HttpGet("{playlistId:guid}")]
    public async Task<ActionResult<PlaylistDto>> GetByIdAsync(Guid playlistId)
    {
        return await _playlistsService.Get(playlistId);
    }

    //POST /api/playlists
    [HttpPost]
    public async Task<ActionResult<PlaylistDto>> PostAsync(PlaylistCreateRequest request)
    {
        var playlist = await _playlistsService.Create(request, GetUserId(), HttpContext);
        var actionName = nameof(GetByIdAsync);
        return CreatedAtAction(actionName, new { playlistId = playlist.Id }, playlist);
    }
    
    //POST /api/playlists
    [HttpPost("{userId:guid}")]
    public async Task<ActionResult<PlaylistDto>> PostAsync(Guid userId, PlaylistCreateRequest request)
    {
        var playlist = await _playlistsService.Create(request, userId, HttpContext);
        var actionName = nameof(GetByIdAsync);
        return CreatedAtAction(actionName, new { playlistId = playlist.Id }, playlist);
    }
    
    //PUT /api/playlists/{playlistId}/title
    [HttpPut("{playlistId:guid}/{title}")]
    public async Task<IActionResult> ChangeTitleAsync(Guid playlistId, [StringValidation(ErrorMessage = DtoErrorMessages.InvalidTitle)] string title)
    {
        await _playlistsService.ChangePlaylistTitle(playlistId, title, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //PUT /api/playlists/{playlistId}/isPublic
    [HttpPut("{playlistId:guid}/{isPublic:bool}")]
    public async Task<IActionResult> ChangePublicationDateAsync(Guid playlistId, bool isPublic)
    {
        await _playlistsService.ChangePlaylistPrivacy(playlistId, isPublic, GetUserId(), HttpContext);
        return NoContent();
    }

    //DELETE /api/playlists/{playlistId}
    [HttpDelete("{playlistId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid playlistId)
    {
        await _playlistsService.Delete(playlistId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //POST /api/playlists/{playlistId}/tracks/{trackId}
    [HttpPost("{playlistId:guid}/Tracks/{trackId:guid}")]
    public async Task<IActionResult> AddTrackToPlaylist(Guid playlistId, Guid trackId)
    {
        await _playlistsService.AddTrackToPlaylist(playlistId, trackId, GetUserId(), HttpContext);
        return NoContent();
    }


    //DELETE /api/playlists/{playlistId}/tracks/{trackId}
    [HttpDelete("{playlistId:guid}/Tracks/{trackId:guid}")]
    public async Task<IActionResult> RemoveTrackFromPlaylist(Guid playlistId, Guid trackId)
    {
        await _playlistsService.RemoveTrackFromPlaylist(playlistId, trackId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //POST /api/playlists/{playlistId}/Tracks/album/{albumToAddId}
    [HttpPost("{playlistId:guid}/Tracks/Album/{albumToAddId:guid}")]
    public async Task<IActionResult> AddTracksFromAlbum(Guid playlistId, Guid albumToAddId)
    {
        await _playlistsService.AddTracksFromAlbum(playlistId, albumToAddId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //POST /api/playlists/{playlistId}/Tracks/playlist/{playlistToAddId}
    [HttpPost("{playlistId:guid}/Tracks/Playlist/{playlistToAddId:guid}")]
    public async Task<IActionResult> AddTracksFromPlaylist(Guid playlistId, Guid playlistToAddId)
    {
        await _playlistsService.AddTracksFromPlaylist(playlistId, playlistToAddId, GetUserId(), HttpContext);
        return NoContent();
    }
    
    //POST /api/playlists/{playlistId}/Tracks/artist/{artsistToAddId}
    [HttpPost("{playlistId:guid}/Tracks/Artist/{artistToAddId:guid}")]
    public async Task<IActionResult> AddTracksFromArtist(Guid playlistId, Guid artistToAddId)
    {
        await _playlistsService.AddTracksFromArtist(playlistId, artistToAddId, GetUserId(), HttpContext);
        return NoContent();
    }
}