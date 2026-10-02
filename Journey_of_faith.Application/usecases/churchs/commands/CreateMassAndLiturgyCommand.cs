using MediatR;

namespace Journey_of_faith.Application.usecases.churchs.commands;

public class CreateMassAndLiturgyCommand : IRequest<Unit>
{
    public List<CreateMassAndLiturgyItem> Items { get; set; } = new();
}

public class CreateMassAndLiturgyItem
{
    public CreateMassScheduleCommand MassScheduleCreate { get; set; } = new();
    public CreateLiturgyCommand LiturgyCreate { get; set; } = new();
}

public class CreateMassScheduleCommand
{
    public int ChurchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? Date { get; set; }
    public string Time { get; set; } = string.Empty;
}

public class CreateLiturgyCommand
{
    public string Reading { get; set; } = string.Empty;
    public string ResponsorialPsalm { get; set; } = string.Empty;
    public string Gospel { get; set; } = string.Empty;
    public string EndWord { get; set; } = string.Empty;
}