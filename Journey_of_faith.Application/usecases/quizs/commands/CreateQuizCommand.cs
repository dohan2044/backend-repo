using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.questions;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class CreateQuizCommand : IRequest<int>
    {
        public string Title { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int TimeLimit { get; set;  }
        public bool IsDaily { get; set; } = true;
        public int QuestionCount { get; set; }
        public int HardQuestion { get; set; }
        public int MediumQuestion { get; set; }
        public int EasyQuestion { get; set; }
    }


    public class CreateQuizQuestionCommand
    {
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int OrderIndex { get; set;  }
    }


    public class CreateQuizValidator : AbstractValidator<CreateQuizCommand>
    {
        private readonly IQuestionQueries _queries;
        private readonly ICurrentUserService _currentUser;
        public CreateQuizValidator(IQuestionQueries questionQueries, ICurrentUserService currentUserService)
        {
            _queries = questionQueries;
            _currentUser = currentUserService;
            RuleFor(e => e.Title)
                .NotEmpty().WithMessage("Tên đề thi không được bỏ trống")
                .MaximumLength(200).WithMessage("Tên đề thi không được vượt quá 200 ký tự");

            RuleFor(e => e.Description)
                .NotEmpty().WithMessage("Mô tả không được bỏ trống");

            RuleFor(e => e.TimeLimit)
                .NotEmpty().WithMessage("Thời gian làm bài không được bỏ trống")
                .GreaterThan(0).WithMessage("Thời gian làm bài không được nhỏ hơn 0");

            RuleFor(x => x)
                .Must(x => x.HardQuestion + x.MediumQuestion + x.EasyQuestion == x.QuestionCount)
                .WithMessage("Tổng các Số lượng Cấp độ câu hỏi không được vượt quá Tổng số câu hỏi trong đề thi");

            RuleFor(e => e.QuestionCount)
                .Must((count, cancellation) =>
                {
                    return _currentUser.GetRoleUserName == "admin";
                }).WithMessage("Bạn chỉ được tạo tối đa 20 câu hỏi trong 1 đề thi.");


            RuleFor(x => x.HardQuestion).GreaterThanOrEqualTo(0).WithMessage("Số câu hỏi khó không được nhỏ hơn 0");
            RuleFor(x => x.HardQuestion).GreaterThanOrEqualTo(0).WithMessage("Số câu hỏi trung bình không được nhỏ hơn 0");
            RuleFor(x => x.HardQuestion).GreaterThanOrEqualTo(0).WithMessage("Số câu hỏi dễ không được nhỏ hơn 0");


            RuleFor(e => e.HardQuestion)
                    .MustAsync(async (hard, token) =>
                    {
                        return await _queries.GetQuestionCountByLevelAsync("Khó", token) >= hard;
                    }).WithMessage("Số lượng câu hỏi khó trong cơ sở dữ liệu không đủ");

            RuleFor(e => e.MediumQuestion)
                    .MustAsync(async (medium, token) =>
                    {
                        return await _queries.GetQuestionCountByLevelAsync("Trung bình", token) >= medium;
                    }).WithMessage("Số lượng câu hỏi trung bình trong cơ sở dữ liệu không đủ");

            RuleFor(e => e.EasyQuestion)
                    .MustAsync(async (easy, token) =>
                    {
                        return await _queries.GetQuestionCountByLevelAsync("Dễ", token) >= easy;
                    }).WithMessage("Số lượng câu hỏi dễ trong cơ sở dữ liệu không đủ");

        }
    }

    public class CreateQuizQuestionValidator : AbstractValidator<CreateQuizQuestionCommand>
    {
        private readonly IQuestionQueries questionQueries;
        public CreateQuizQuestionValidator(IQuestionQueries questionQueries)
        {

            this.questionQueries = questionQueries;
            RuleFor(e => e.QuizId).NotEmpty().WithMessage("Mã đề kh đucợ để trống");
            RuleFor(e => e.QuestionId).NotEmpty().WithMessage("questionId không được để trống")
                .MustAsync(async (id, cancellationToken) =>
            {
                return await questionQueries.IdExistsAsync(id, "Question", cancellationToken);
            }).WithMessage("Question ID Không hợp lệ");
        }
    }


}
