using Asp.Versioning;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.common.dtos.songs;
using Journey_of_faith.Application.usecases.songs.commands;
using Journey_of_faith.Application.usecases.songs.queries;
using Journey_of_faith.Domain.dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Journey_of_faith.Api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("api/v{version:apiVersion}/songs")]
public sealed class SongController : ControllerBase
{
    private readonly IMediator _mediator;

    public SongController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSongs([FromQuery] GetSongsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(new ApiResponse<PagedResult<SongDto>>
        {
            Message = result.TotalCount > 0 ? "Lấy danh sách bài hát thành công." : "Không có bài hát phù hợp.",
            Data = result
        });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSong([FromRoute] int id)
    {
        var result = await _mediator.Send(new GetSongQuery(id));
        return result is null
            ? NotFound(new ApiResponse<object> { Message = "Không tìm thấy bài hát.", Data = new { Id = id } })
            : Ok(new ApiResponse<SongDto> { Message = "Lấy chi tiết bài hát thành công.", Data = result });
    }

    [HttpPost]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSong([FromBody] CreateSongCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSong), new { id, version = "1" },
            new ApiResponse<int> { Message = "Tạo bài hát thành công.", Data = id });
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateSong([FromRoute] int id, [FromBody] UpdateSongCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return Ok(new ApiResponse<object> { Message = "Cập nhật bài hát thành công.", Data = new { Id = id } });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteSong([FromRoute] int id)
    {
        var deleted = await _mediator.Send(new DeleteSongCommand { Id = id });
        if (!deleted)
            return NotFound(new ApiResponse<object> { Message = "Không tìm thấy bài hát.", Data = new { Id = id } });
        return Ok(new ApiResponse<object> { Message = "Xóa bài hát thành công.", Data = new { Id = id } });
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _mediator.Send(new GetSongCategoriesQuery());
        return Ok(new ApiResponse<IReadOnlyList<SongCategoryDto>>
        {
            Message = "Lấy danh sách thể loại bài hát thành công.",
            Data = result
        });
    }

    [HttpPost("categories")]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateSongCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created,
            new ApiResponse<int> { Message = "Tạo thể loại bài hát thành công.", Data = id });
    }

    [HttpPut("categories/{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateSongCategoryCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return Ok(new ApiResponse<object> { Message = "Cập nhật thể loại bài hát thành công.", Data = new { Id = id } });
    }

    [HttpDelete("categories/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        var deleted = await _mediator.Send(new DeleteSongCategoryCommand { Id = id });
        if (!deleted)
            return NotFound(new ApiResponse<object> { Message = "Không tìm thấy thể loại bài hát.", Data = new { Id = id } });
        return Ok(new ApiResponse<object> { Message = "Xóa thể loại bài hát thành công.", Data = new { Id = id } });
    }

    [HttpGet("category-mappings")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryMappings([FromQuery] GetSongCategoryMappingsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(new ApiResponse<IReadOnlyList<SongCategoryMappingDto>>
        {
            Message = "Lấy danh sách liên kết bài hát - thể loại thành công.",
            Data = result
        });
    }

    [HttpPost("category-mappings")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCategoryMapping([FromBody] CreateSongCategoryMappingCommand command)
    {
        var id = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created,
            new ApiResponse<int> { Message = "Gắn thể loại cho bài hát thành công.", Data = id });
    }

    [HttpDelete("{songId:int}/categories/{categoryId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategoryMapping([FromRoute] int songId, [FromRoute] int categoryId)
    {
        await _mediator.Send(new DeleteSongCategoryMappingCommand
        {
            SongId = songId,
            CategoryId = categoryId
        });
        return Ok(new ApiResponse<object>
        {
            Message = "Gỡ thể loại khỏi bài hát thành công.",
            Data = new { SongId = songId, CategoryId = categoryId }
        });
    }
}
