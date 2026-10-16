using Journey_of_faith.Application.common.caching.interfaces;
using Journey_of_faith.Application.common.dtos.church;
using Journey_of_faith.Application.common.untils;
using Journey_of_faith.Application.usecases.churchs;
using Journey_of_faith.Domain.dtos;
using MediatR;
using OfficeOpenXml.Drawing.Chart;

public class GetChurchWithCondition : IRequest<PagedResult<ChurchViewDto>>, ICacheableQuery
{
    public string? NameChurch { get; set; }
    public string? Province { get; set; }
    public string? Ward { get; set; }
    public string? Time { get; set; }
    public String? Name { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double Longitude {get; set;}
    public double Latitude {get; set;}
    public string CacheGroup => "churches";
    public string CacheKey => $"churches:{NameChurch}_{Province}_{Ward}_{Time}_{Page}_{PageSize}_{Longitude:F$}_{Latitude:F4}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}

public record DataInstance(int ChurchId, double Longitude, double Latitude, double Instance);
public class GetChurchWithConditionHandler : IRequestHandler<GetChurchWithCondition, PagedResult<ChurchViewDto>>
{
    private readonly IChurchQueries churchQueries;

    public GetChurchWithConditionHandler(IChurchQueries churchQueries)
    {
        this.churchQueries = churchQueries;
    }

    public async Task<PagedResult<ChurchViewDto>> Handle(
        GetChurchWithCondition query,
        CancellationToken cancellationToken)
    {
        var data = await churchQueries.GetListAsync(new QueryFilter
        {
            ChurchName = query.NameChurch,
            Province = query.Province,
            Ward = query.Ward,
            Time = query.Time,
            Page = query.Page,
            PageSize = query.PageSize,
        });
        foreach(var church in data.Data)
        {
            if(church.Latitude is not null || church.Longitude is not null) 
                church.Instance = $"{CalculateDistance(
                    query.Latitude, 
                    query.Longitude, 
                    church.Latitude ?? default, 
                    church.Longitude ?? default).FormatNumbeDoubleToString("f2")} km";
        }
        data.Data = data.Data.OrderBy(e => e.Instance).ToList();
        return data;
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371; // Bán kính Trái Đất (km)

        // Chuyển đổi độ (Degrees) sang Radian
        double dLat = ToRadian(lat2 - lat1);
        double dLon = ToRadian(lon2 - lon1);

        double rLat1 = ToRadian(lat1);
        double rLat2 = ToRadian(lat2);

        // Áp dụng công thức Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(rLat1) * Math.Cos(rLat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Trả về khoảng cách theo km
    }

    private static double ToRadian(double angle)
    {
        return angle * Math.PI / 180.0;
    }
}
