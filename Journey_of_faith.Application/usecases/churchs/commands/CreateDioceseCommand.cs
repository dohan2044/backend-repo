using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.churchs.commands
{
    public class CreateDioceseCommand : IRequest<int>
    {
        public required string Name { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
        public string? Thumbnail { get; set; }
        public Guid CreatorUserId { get; set; }
        public Guid LastModificationTime { get; set; }
    }

    public class CreateDioceseValidator : AbstractValidator<CreateDioceseCommand>
    {
        public CreateDioceseValidator()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("Tên Giáo phận không được để trống");
        }
    }
}
