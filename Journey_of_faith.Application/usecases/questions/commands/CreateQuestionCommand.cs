using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.questions.commands
{
    public class CreateQuizLevelCommand : IRequest<bool>
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
    }

    public class CreateQuestionTypeCommand : IRequest<bool>
    {
        public string Name {get; set;} = string.Empty;
        public string Code { get; set;} = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class CreateQuestionCategoryCommand : IRequest<bool>
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }


    public class CreateQuestionCommand : IRequest<bool>
    {
        public int LevelId { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName {get; set;}
        public string? ImageUrl { get; set; }
        public List<CreateAnswerCommand> Items { get; set; }
    }

    public class CreateAnswerCommand
    {
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? Explanation { get; set; } = string.Empty;
    }

    // validate
    public class CreateQuizLevelValidator : AbstractValidator<CreateQuizLevelCommand>
    {
        public CreateQuizLevelValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Tên Cấp độ không được để trống.");
        }
    }

    public class CreateQuestionTypeValidator : AbstractValidator<CreateQuestionTypeCommand>
    {
        public CreateQuestionTypeValidator() { RuleFor(e => e.Name).NotEmpty().WithMessage("Tên Kiểu câu hỏi không được để trống."); }
    }

    public class CreateQuestionCategoryValidator : AbstractValidator<CreateQuestionCategoryCommand>
    {
        public CreateQuestionCategoryValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Tên chủ đề câu hỏi không được để trống");
        }
    }

    public class CreateAnswerValidator : AbstractValidator<CreateAnswerCommand>
    {
        public CreateAnswerValidator()
        {
            RuleFor(e => e.QuestionId)
                .NotEmpty().WithMessage("Mã câu hỏi không được trống")
                .NotNull().WithMessage("Mã câu hỏi không được phép null")
                .GreaterThan(0).WithMessage("Mã câu hỏi phải lớn hơn 0");

            RuleFor(e => e.Content)
                .NotEmpty().WithMessage("Nội dung đáp án kh được để trống")
                .MaximumLength(300).WithMessage("Đáp án không được vượt quá 300 ký tự");

            RuleFor(e => e.IsCorrect)
                .NotEmpty().WithMessage("Không được để trống caasi trường IsCorrect");
        }
    }
    public class CreateQuestionValidator : AbstractValidator<CreateQuestionCommand>
    {
        public CreateQuestionValidator()
        {
            RuleFor(e => e.LevelId)
                .NotEmpty().WithMessage("Mã Cấp độ câu hỏi không được để trống")
                .GreaterThan(0).WithMessage("Mã Cấp độ không được nhỏ hơn 0");
            RuleFor(e => e.QuestionContent)
                .NotEmpty().WithMessage("Nột dung câu hỏi không được để trống")
                .MaximumLength(500).WithMessage("Nội dung câu hỏi không được vượt quá 500 ký tự.");
            RuleFor(e => e.TypeId)
                .NotEmpty().WithMessage("Mã kiểu câu hỏi không được để trống")
                .GreaterThan(0).WithMessage("Mã Kiểu câu hỏi không được nhỏ hơn 0");


            RuleFor(e => e.CategoryId)
               .NotEmpty().WithMessage("Danh mục câu hỏi không được để trống")
               .GreaterThan(0).WithMessage("Danh mục không được nhỏ hơn 0");

            RuleForEach(e => e.Items).SetValidator(new CreateAnswerValidator());
        }
    }
}
