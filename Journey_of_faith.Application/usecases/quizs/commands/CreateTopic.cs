using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class CreateTopicCommand : IRequest<int>
    {
        public string TopicName { get; set; } = string.Empty;
        public int QuizCount { get; set; }
    }

    public class CreateTopicValidator : AbstractValidator<CreateTopicCommand>
    {
        public CreateTopicValidator()
        {
            RuleFor(e => e.TopicName).NotEmpty()
                .WithMessage("Tên Chủ để không được để trống.");

            RuleFor(e => e.QuizCount)
                .GreaterThan(0).WithMessage("Số đề thi trong chủ đề phải lớn hơn 0");
        }
    }
}
