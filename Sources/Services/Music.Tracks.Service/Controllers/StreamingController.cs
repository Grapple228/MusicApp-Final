using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Tracks.Service.Services;

namespace Music.Tracks.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StreamingController : ControllerBase
{
    private readonly ITracksService _tracksService;

    public StreamingController(ITracksService tracksService)
    {
        _tracksService = tracksService;
    }

    [HttpGet("{trackId:guid}")]
    public async Task<FileResult> Listen(Guid trackId)
    {
        return await _tracksService.GetTrackFile(trackId);
    }
    
    
}