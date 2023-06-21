using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Genres.Service.Services;
using Music.Services.Identity.Common;
using Music.Shared.Common;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Requests.Genres;

namespace Music.Genres.Service.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenresService _genresService;

    public GenresController(IGenresService genresService)
    {
        _genresService = genresService;
    }

    //GET /api/genres
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto?>>> GetAsync()
    {
        return Ok(await _genresService.GetAll());
    }

    //GET /api/genres/{pageNumber}-{countPerPage}
    [HttpGet("{pageNumber:long}-{countPerPage:long}")]
    public async Task<ActionResult<PageResult<GenreDto>>> GetAsync([Range(1, long.MaxValue)] long pageNumber, [Range(1, long.MaxValue)] long countPerPage)
    {
        return await _genresService.GetPage(pageNumber, countPerPage);
    }
    
    //GET /api/genres/{genreId}
    [HttpGet("{genreId:guid}")]
    public async Task<ActionResult<GenreDto>> GetByIdAsync(Guid genreId)
    {
        return await _genresService.Get(genreId);
    }

    //POST /api/genres
    [HttpPost]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<ActionResult<GenreDto>> PostAsync(GenreCreateRequest request)
    {
        var genre = await _genresService.Create(request);
        var actionName = nameof(GetByIdAsync);
        return CreatedAtAction(actionName, new { genreId = genre.Id }, genre);
    }

    //PUT /api/genres/{genreId}
    [HttpPut("{genreId:guid}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<ActionResult<GenreDto>> PutAsync(Guid genreId, GenreUpdateRequest request)
    {
        return await _genresService.Update(genreId, request);
    }

    //DELETE /api/genres/{genreId}
    [HttpDelete("{genreId:guid}")]
    [Authorize(Policy = Policies.AdminPolicy)]
    public async Task<IActionResult> DeleteAsync(Guid genreId)
    {
        await _genresService.Delete(genreId);
        return NoContent();
    }
}