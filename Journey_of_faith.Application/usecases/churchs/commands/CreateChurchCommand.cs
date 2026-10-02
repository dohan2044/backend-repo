using FluentValidation;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.entities.masslive;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Journey_of_faith.Application.usecases.churchs.commands
{
    public class CreateChurchCommand : IRequest<int>, ICacheInvalidCommand
    {
        public string Name { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public string? Boss {get; set;}
        public string? Website { get; set; }
        public string? Address { get; set; }
        public int DioceseId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Guid CreatorUser { get; set; }
        public Guid LastModifierUserId { get; set; }
        public List<MassSchedule>? MassSchedules {get; set;}
        public List<string> ChurchImages { get; set; } = [];
        public List<IFormFile>? Files { get; set; }
        public string? Description {get; set;}

        public string[] CacheKeys => ["churches-name"];
    }

    public class MassScheduleItem
    {
        public string Name {get; set;}
        public string Time {get; set;}
    }

    public class CreateChurchCommandValidator : AbstractValidator<CreateChurchCommand>
    {
        public CreateChurchCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên nhà thờ không được để trống.")
                .MaximumLength(255).WithMessage("Tên nhà thờ không được vượt quá 255 ký tự.");
            RuleFor(x => x.Thumbnail)
                .MaximumLength(500).WithMessage("Đường dẫn hình ảnh không được vượt quá 500 ký tự.");
            RuleFor(x => x.Website)
                .MaximumLength(500).WithMessage("Đường dẫn website không được vượt quá 500 ký tự.");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Địa chỉ nhà thờ không được để trống.")
                .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự.");
            RuleFor(x => x.DioceseId)
                .NotEmpty().WithMessage("Id giáo phận không được để trống.")
                .GreaterThan(0).WithMessage("Id giáo phận phải lớn hơn 0.");
            RuleFor(x => x.Latitude)
                .NotEmpty().WithMessage("Vĩ độ không được để trống.")
                .InclusiveBetween(-90, 90).WithMessage("Vĩ độ phải nằm trong khoảng -90 đến 90.");
            RuleFor(x => x.Longitude)
                .NotEmpty().WithMessage("Kinh độ không được để trống.")
                .InclusiveBetween(-180, 180).WithMessage("Kinh độ phải nằm trong khoảng -180 đến 180.");
        }
    }
}
