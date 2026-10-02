using Asp.Versioning;
using Journey_of_faith.Api.Attributes;
using Journey_of_faith.Api.authorization;
using Journey_of_faith.Api.dtos;
using Journey_of_faith.Application.usecases.quizs.commands;
using Journey_of_faith.Application.usecases.quizs.queries;
using Journey_of_faith.Application.common.dtos.quiz;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Mime;

namespace Journey_of_faith.Api.Controllers
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{version:apiVersion}/quiz")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [HasPermission(Permissions.Quizes.CREATE)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizCommand command)
        {
            var quiz = await _mediator.Send(command);
            return StatusCode(statusCode: StatusCodes.Status201Created, new
            {
                Message = "Tạo đề thi thành công",
                Data = quiz
            });
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QuizViewDto>>), StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetAll()
        {
            var quizzes = await _mediator.Send(new GetAllQuizzesQuery());
            return Ok(new ApiResponse<IEnumerable<QuizViewDto>>
            {
                Message = "Lấy danh sách đề thi thành công",
                Data = quizzes
            });
        }
        [HttpGet("{id}/details")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await _mediator.Send(new GetDetailsQuizQuery { Id = id });
            if(details is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Không có dữ liệu",
                    Data = details
                });
            }
            return Ok(new ApiResponse<QuizViewDto>
            {
                Message = "Lấy dữ liệu thành công",
                Data = details
            });
        }
        [HttpPost("submit")]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> SubmitExam(SubmitExamCommand command)
        {
            var com = await _mediator.Send(command);
            return Ok(new ApiResponse<SubmitResult>
            {
                Message = "Nọp bài thành công.",
                Data = com
            });
        }
        [HttpGet("history-test")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetHistoryTest()
        {
            var data = await _mediator.Send(new GetHistoryExamTestQuery());
            return Ok(new ApiResponse<IReadOnlyList<ExamHistoryDto>>
            {
                Message = "lay du lieu thanh cong",
                Data = data
            });
        }
        [HttpDelete("{id}")]
        [HasPermission(Permissions.Quizes.DELETE)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            await _mediator.Send(new DeleteQuizCommand { Id = id });
            return NoContent();
        }

        [HttpGet("topics")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> GetTopics()
        {
            var topics = await _mediator.Send(new GetAllTopicQuery());
            return Ok(new ApiResponse<IEnumerable<TopicViewDto>>
            {
                Data = topics,
                Message = "Lấy chủ thế thành công"
            });
        }
        [HttpPost("topics")]
        [HasPermission(Permissions.Quizes.CREATE)]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> CreateTopicAsync([FromBody] CreateTopicCommand command)
        {
            await _mediator.Send(command);
            return StatusCode(statusCode: StatusCodes.Status201Created, new
            {
                Message = "Tạo mới chủ đề thành công."
            });
        }
        [HttpDelete("topics")]
        [HasPermission(Permissions.Quizes.DELETE)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [MapToApiVersion(1)]
        public async Task<IActionResult> DeleteTopic([FromBody] int id)
        {
            await _mediator.Send(new DeleteTopicCommand { Id = id });
            return NoContent();
        }
    }
}
