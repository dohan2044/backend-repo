using Google.Apis.Http;

namespace Journey_of_faith.Infrastructure.services;


public class TimeoutHttpClientFactory : HttpClientFactory, IConfigurableHttpClientInitializer
{
    private readonly TimeSpan _timeout;

    public TimeoutHttpClientFactory(TimeSpan timeout)
    {
        _timeout = timeout;
    }

    // Hàm này của Interface sẽ tự động chạy ngay sau khi Google tạo xong HttpClient
    public void Initialize(ConfigurableHttpClient httpClient)
    {
        httpClient.Timeout = _timeout; // Ghi đè cấu hình Timeout 100s gốc của Google
    }

    // Ghi đè hàm khởi tạo chuẩn để ép nạp cái Initializer này vào danh sách cấu hình chạy ngầm
    public new ConfigurableHttpClient CreateHttpClient(CreateHttpClientArgs args)
    {
        // Thêm chính nó vào danh sách cấu hình khởi tạo của Google
        args.Initializers.Add(this);
        return base.CreateHttpClient(args);
    }
}