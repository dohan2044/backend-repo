using FluentValidation;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class DeleteTopicCommand : IRequest<int>
    {
        public int Id { get; set; }
    }


    public class DeleteTopicValidator : AbstractValidator<DeleteTopicCommand>
    {
        public DeleteTopicValidator()
        {
            RuleFor(e => e.Id).GreaterThan(0).WithMessage("Mã phải lớn 0");
        }
    }

    public class DeleteTopicHandler : IRequestHandler<DeleteTopicCommand, int>
    {
        private readonly IExamRepository examRepository;
        public DeleteTopicHandler(IExamRepository examRepository)
        {
            this.examRepository = examRepository;
        }

        public async Task<int> Handle(DeleteTopicCommand command, CancellationToken cancellationToken)
        {
            return await examRepository.DeleteTopicAsync(command.Id);
        }
    }
}
