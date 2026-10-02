# Đặt lịch gửi Firebase bằng Quartz

API nhận lịch từ request của backend/admin; Quartz chạy nền trong tiến trình API,
đến giờ gọi `IFirebaseNotification` để gửi Firebase Cloud Messaging (FCM).
Không cần request HTTP mới khi đến giờ chạy. Backend phải đang hoạt động để gửi.

## Chuẩn bị

1. Khôi phục NuGet: `dotnet restore Journey_of_faith.Api/Journey_of_faith.Api.csproj`.
2. Trong SQL Server, chọn đúng database và chạy `scripts/sql/quartz-sqlserver.sql`
   một lần trước khi khởi động API. Script chỉ tạo các bảng `dbo.QRTZ_*`, từ chối
   chạy nếu đã có bảng Quartz, không xóa dữ liệu. Không phải migration EF Core.
3. Quartz mặc định dùng `ConnectionStrings:Connection`. Có thể đặt
   `ConnectionStrings:Quartz` qua user-secrets/biến môi trường để dùng database riêng.
   Tài khoản runtime cần quyền đọc/ghi trên các bảng Quartz; tài khoản khởi tạo cần
   quyền tạo bảng trong schema `dbo`.
4. Giữ cấu hình service account `Firebase` đang được `AddFirebaseService` sử dụng
   (project_id, private_key, client_email và các trường hiện có). Không đưa khóa thật
   vào request đặt lịch. FCM phải được bật; app nhận phải có token hợp lệ hoặc đã
   subscribe topic tương ứng.
5. Gọi API với `Authorization: Bearer <access_token>` của admin. Policy chấp nhận
   claim `role` hoặc `ClaimTypes.Role` có giá trị `admin` (không phân biệt hoa thường).

Quartz 3.18.0 dùng SQL Server persistent store và clustering: lịch đã lưu tồn tại qua
restart. Các instance chia sẻ cùng database và scheduler name sẽ cùng điều phối lịch.
Cần đồng bộ đồng hồ giữa các server. Không đổi scheduler name khi muốn tiếp tục lịch cũ.

Chỉ khi chạy thử local không cần lưu lịch, đặt biến môi trường
`NotificationScheduling__UsePersistentStore=false`. Chế độ RAM mất toàn bộ lịch khi
restart và không chia sẻ lịch giữa các instance. Mặc định là `true`; thiếu các bảng
Quartz sẽ khiến scheduler không khởi động, không tự chuyển về RAM.

## API

| Method | Endpoint | Kết quả |
| --- | --- | --- |
| POST | `/api/v1/notifications/schedules` | `201`, thông tin lịch và header `Location` |
| GET | `/api/v1/notifications/schedules/{id}` | `200`, giờ chạy tiếp theo/trạng thái Quartz; `404` nếu không còn lịch |
| DELETE | `/api/v1/notifications/schedules/{id}` | `204` nếu hủy thành công; `404` nếu không còn lịch |

Request không hợp lệ trả `400`; chưa xác thực trả `401`; không phải admin trả `403`.
Phải cung cấp đúng một nơi nhận (`token` hoặc `topic`) và đúng một kiểu lịch
(`runAt` hoặc `cronExpression`). `title`/`body` bắt buộc. `data` tùy chọn, chỉ gồm
chuỗi và không dùng khóa dành riêng của FCM. API kiểm tra giới hạn kích thước payload
một cách bảo thủ trước khi lưu: 2048 byte cho topic và 4096 byte cho token.

Ví dụ gửi tới topic mỗi ngày lúc 20:30 giờ Việt Nam:

```http
POST /api/v1/notifications/schedules
Authorization: Bearer <admin-access-token>
Content-Type: application/json

{
  "topic": "daily-prayer",
  "title": "Nhắc giờ cầu nguyện",
  "body": "Đã đến giờ cầu nguyện buổi tối.",
  "cronExpression": "0 30 20 * * ?",
  "timeZoneId": "Asia/Ho_Chi_Minh",
  "data": {
    "click_action": "OPEN_PRAYER"
  }
}
```

`timeZoneId` mặc định `Asia/Ho_Chi_Minh`; có thể dùng `UTC` hoặc múi giờ được .NET
hỗ trợ. Cron Quartz có 6 hoặc 7 trường, bắt đầu bằng giây. Ví dụ `0 0 7 * * ?`
là 07:00 hằng ngày, `0 0/30 * * * ?` là mỗi 30 phút.

Ví dụ gửi một lần tới thiết bị (thay `runAt` bằng thời điểm tương lai):

```json
{
  "token": "<fcm-device-token>",
  "title": "Nhắc lịch tham dự",
  "body": "Sự kiện sắp bắt đầu.",
  "runAt": "2030-01-01T18:00:00+07:00",
  "data": {
    "click_action": "OPEN_EVENT",
    "eventId": "123"
  }
}
```

`runAt` bắt buộc có `Z` hoặc UTC offset như `+07:00`; thời điểm đã qua bị từ chối.
Lịch một lần dùng offset trong `runAt`, không dùng `timeZoneId`. Response trả thời
điểm UTC (`runAtUtc`, `nextRunAtUtc`); cron không có lần chạy tương lai bị từ chối.
Nếu muốn đổi lịch/nội dung, hủy ID cũ rồi tạo lịch mới. Mỗi POST tạo một ID mới;
gửi lặp lại cùng request sẽ tạo thêm lịch.

## Hành vi khi chạy

- Một lần: gửi một lượt; nếu bị lỡ do backend tắt, Quartz gửi khi backend hoạt động
  lại và xử lý lịch trễ. Sau khi job kết thúc, Quartz tự dọn job/trigger, GET trả 404.
- Cron: các lượt quá ngưỡng misfire (mặc định Quartz khoảng 60 giây) được bỏ qua;
  tiếp tục lượt kế tiếp. Một lượt trễ dưới ngưỡng vẫn có thể chạy muộn.
- Cùng một job không chạy đồng thời. Các lịch khác nhau có thể chạy song song.
- DELETE ngăn các lượt chạy tiếp theo, không thu hồi thông báo đã gửi hoặc đảm bảo
  dừng một job đã bắt đầu chạy.
- Khi FCM lỗi, job ghi log và báo lỗi cho Quartz; không retry ngay. Lịch cron vẫn
  tiếp tục ở lượt kế tiếp. Lịch một lần lỗi cần tạo lại nếu muốn thử lại.
- `State` là trạng thái trigger Quartz, không phải xác nhận thiết bị đã nhận tin.
  Log thành công chỉ cho biết FCM đã chấp nhận message. Chưa có bảng lịch sử gửi,
  retry bền vững hay bảo đảm exactly-once. Sự cố ngay trong lúc gửi có thể mất một
  lượt hoặc tạo kết quả không xác định; phía app nên chống trùng nếu nghiệp vụ cần.
- Token FCM được giữ nguyên từ lúc đặt lịch. Nếu app đổi token, cần đặt lại lịch
  cho token mới; với nhắc chung dài hạn có thể dùng topic.

## Kiểm tra

```powershell
dotnet test UnitTesting/UnitTesting.csproj --filter FullyQualifiedName~UnitTesting.Scheduling
dotnet test IntegrationTesting/IntegrationTesting.csproj --filter FullyQualifiedName~NotificationSchedulesControllerTests -p:OutputPath=bin/QuartzValidation/
```

Các test trên dùng scheduler RAM riêng và Firebase giả lập. Test HTTP chỉ khởi động
TestServer riêng, không chạy `Program` hoặc đọc appsettings thật. Không dùng database
hoặc Firebase thật. Chỉ dùng filter trên cho nhóm HTTP này; các integration test cũ
của project không có cùng cơ chế cô lập. Khi API đang chạy giữ DLL, có thể build
kiểm tra sang thư mục khác:

```powershell
dotnet build Journey_of_faith.Api/Journey_of_faith.Api.csproj --no-restore -p:OutputPath=bin/QuartzValidation/
```

Tài liệu tham chiếu: [Quartz DI và persistent store](https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html),
[Quartz cron](https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html),
[Firebase Admin gửi thông báo](https://firebase.google.com/docs/cloud-messaging/send/admin-sdk).
