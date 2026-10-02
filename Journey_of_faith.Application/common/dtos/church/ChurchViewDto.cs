namespace Journey_of_faith.Application.common.dtos.church;

public class ChurchViewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int DioceseId { get; set; }
    public string? DioceseName { get; set; }
    public string Boss { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public bool IsFollowed { get; set; }
    public IEnumerable<MassScheduleViewDto> MassSchedules { get; set; } = [];
    public IEnumerable<ChurchImageViewDto> ChurchImages { get; set; } = [];
}
