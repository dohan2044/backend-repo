using Journey_of_faith.Application.usecases.questions.commands;
using Journey_of_faith.Application.usecases.questions.queries;
using Journey_of_faith.Application.common.dtos;
using Journey_of_faith.Domain.entities.quiz;
using MediatR;
using Journey_of_faith.Api.dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Application.exceptions;
using Asp.Versioning;
using Journey_of_faith.Api.Attributes;
using Journey_of_faith.Api.authorization;

#nullable disable
namespace Journey_of_faith.Api.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{version:apiVersion}/questions")]
    public sealed class QuestionController(IMediator mediator, IFileStorageService service) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IFileStorageService _fileStorageService = service;
        /// <summary>
        /// quizLevel => questionLevel
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("question-levels")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> LevelCreate([FromBody] CreateQuizLevelCommand command)
        {
            var status = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new
            {
                Message = "Tạo thành công",
                Status = status
            });
        }

        [HttpGet("question-levels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetAllLevel()
        {
            var result = await _mediator.Send(new GetLevelQuery());
            if (!result.Any())
            {
                return Ok(new ApiResponse<IEnumerable<QuizLevelDto>>
                {
                    Message = "Không dó dữ liệu",
                    Data = result
                });
            }
            return Ok(new ApiResponse<IEnumerable<QuizLevelDto>>
            {
                Message = "Lấy dữ liệu thành công",
                Data = result
            });

        }
        [HttpGet("question-levels/{levelId}/details")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetDetailsQuiz([FromRoute] int levelId)
        {
            var quiz = await _mediator.Send(new GetDetailsQuestionLevelCommand { Id = levelId });
            if (quiz is null)
            {
                return NotFound(new ApiResponse<QuizLevelDto>
                {
                    Message = "Không tìm thấy dữ liệu",
                    Data = quiz
                });
            }
            return Ok(new ApiResponse<QuizLevelDto>
            {
                Message = "Lấy dữ liệu thành công",
                Data = quiz
            });
        }

        /// <summary>
        ///  Create: Question Type
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("question-types")]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission(Permissions.Questions.CREATE)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> QuestionTypeCreated([FromBody] CreateQuestionTypeCommand command)
        {
            var status = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new
            {
                Message = "Tạo thành công",
                Status = status
            });
        }
        [HttpGet("question-types")]
        [HasPermission(Permissions.Questions.VIEW)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetAllType()
        {
            var types = await _mediator.Send(new GetQuestionTypeQuery());
            if (!types.Any())
            {
                return Ok(new ApiResponse<IEnumerable<QuestionTypeDto>>
                {
                    Message = "Không có dữ liệu",
                    Data = types
                });
            }

            return Ok(new ApiResponse<IEnumerable<QuestionTypeDto>>
            {
                Message = "Lấy dữ lieuj thành công.",
                Data = types
            });
        }
        [HttpGet("question-types/{id}/details")]
        [HasPermission(Permissions.Questions.VIEW)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetDetailsType([FromRoute] int id)
        {
            var type = await _mediator.Send(new GetDetailsQuestionTypeQuery { Id = id });
            if (type is null)
            {
                return NotFound(new ApiResponse<QuestionTypeDto>
                {
                    Message = "Không có dữ liệu",
                    Data = type
                });
            }
            return Ok(new ApiResponse<QuestionTypeDto>
            {
                Message = "Lấy dữ liệu thành công",
                Data = type
            });
        }

        /// <summary>
        /// Create Question category
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("question-categories")]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission(Permissions.Questions.CREATE)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> QuestionCategoryCreated([FromBody] CreateQuestionCategoryCommand command)
        {
            var status = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new
            {
                Message = "Tạo thành công",
                Status = status
            });
        }
        [HttpGet("question-categories")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await _mediator.Send(new GetCategoriesQuery());
            if (!categories.Any())
            {
                return Ok(new ApiResponse<IEnumerable<QuestionCategoryDto>>
                {
                    Message = "Không dó dữ liệu",
                    Data = categories
                });
            }
            return Ok(new ApiResponse<IEnumerable<QuestionCategoryDto>>
            {
                Message = "Lấy dữ liệu thành công",
                Data = categories
            });
        }

        [HttpGet("question-categories/{id}/details")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetDetailsCategory([FromRoute] int id)
        {
            var category = await _mediator.Send(new GetDetailsQuestionCategoryQuery { Id = id });
            if (category is null)
            {
                return NotFound(new ApiResponse<QuestionCategoryDto>
                {
                    Message = "Không có dữ liệu",
                    Data = category
                });
            }
            return Ok(new ApiResponse<QuestionCategoryDto>
            {
                Message = "Lấy dữ liệu thành công.",
                Data = category
            });
        }
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission(Permissions.Questions.CREATE)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionCommand command)
        {
            // if(file != null)
            // {
            //     var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "uploads", "question");
            //     if(!Directory.Exists(path))
            //     {
            //         Directory.CreateDirectory(path);
            //     }

            //     var UniquiFile = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";
            //     var filePath = System.IO.Path.Combine(path, UniquiFile);

            //     using(var stream = new FileStream(filePath, FileMode.Create))
            //     {
            //         await file.CopyToAsync(stream);
            //     }

            //     command.ImageUrl = System.IO.Path.Combine("uploads", "question", UniquiFile);
            // }
            await _mediator.Send(command);
            return Ok(new ApiResponse<object>
            {
                Message = "Tạo Câu hỏi thành công",
                Data = command
            });
        }
        [HttpGet]
        [HasPermission(Permissions.Questions.VIEW)]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetQuestions([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? search)
        {
            var data = await _mediator.Send(new GetQuestionsQuery { Page = page, PageSize = pageSize, Search = search });
            return Ok(data);
        }

        [HttpPut]
        [HasPermission(Permissions.Questions.Edit)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
        [HttpDelete("{Id}")]
        [HasPermission(Permissions.Questions.DELETE)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> DeleteQuestion(int Id)
        {
            await _mediator.Send(new DeleteQuestionCommand { Id = Id });
            return NoContent();
        }
        [HttpGet("template")]
        [HasPermission(Permissions.Questions.CREATE)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetTemplateUpload()
        {
            var template = await _mediator.Send(new GetTemplateUploadFileQuery());
            return Ok(template);
        }

        [HttpPost("upload")]
        [HasPermission(Permissions.Questions.CREATE)]
        [Consumes("multipart/form-data")]
        [MapToApiVersion(1)]
        public async Task<IActionResult> UploadQuestion([FromForm] IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                return BadRequest("File không được để trống");

            var expectedExtension = new[] { ".xls", ".xlsx" };
            var ext = System.IO.Path.GetExtension(formFile.FileName)?.ToLowerInvariant();
            if (!expectedExtension.Contains(ext))
                return BadRequest("File không hợp lệ, vui lòng kiểm tra lại xem có đúng file excel hay không");

            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            try
            {
                await _mediator.Send(new UploadFileCommand { FileBytes = memoryStream.ToArray() });
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("filter-condition")]
        [HasPermission(Permissions.Questions.VIEW)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetQuestionCondition(
            [FromQuery] int CategoryId,
            [FromQuery] int LevelId,
            [FromQuery] int QuestionCount
        )
        {
            var result = await _mediator.Send(new GetQuestionWithConditionQuery {CategoryId = CategoryId, LevelId = LevelId, QuestionCount = QuestionCount});
            return Ok(new ApiResponse<IEnumerable<QuestionView>>
            {
                Data = result,
                Message = "Lấy câu hỏi thành công."
            });
        }
    }
}



public static class StartNameApi
{
    public const string name = "Journey_of_faith";
}
