using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.untils;
using Journey_of_faith.Application.usecases.churchs;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Journey_of_faith.Application.usecases.churchs.commands
{
    public class CreateChurchHandler : IRequestHandler<CreateChurchCommand, int>
    {
        private static readonly string[] Scopes = { DriveService.Scope.Drive };
        private readonly IChurchRepository _repo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChurchQueries _churchQueries;

        public CreateChurchHandler(
            IChurchRepository repo,
            ICurrentUserService currentUserService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IChurchQueries churchQueries)
        {
            _repo = repo;
            _currentUserService = currentUserService;
            this.configuration = configuration;
            this._unitOfWork = unitOfWork;
            _churchQueries = churchQueries;
        }

        public async Task<int> Handle(CreateChurchCommand command, CancellationToken token)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Người dùng không hợp lệ");
            }
            if (!await _churchQueries.DioceseExistsAsync(command.DioceseId, token))
            {
                throw new NotFoundException("Không có giáo phận mà nhà nhờ đăng ký.");
            }

            var settings = configuration.GetSection("GoogleDrive");
            string Required(string key) =>
                !string.IsNullOrWhiteSpace(settings[key])
                    ? settings[key]!.Trim()
                    : throw new InvalidOperationException($"Thiếu cấu hình GoogleDrive:{key}.");

            var parentFolderId = Required("ParentFolderId");
            using var flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = Required("ClientId"),
                        ClientSecret = Required("ClientSecret")
                    },
                    Scopes = Scopes
                });
            var credential = new UserCredential(flow, "church-drive-owner",
                new TokenResponse { RefreshToken = Required("RefreshToken") });
            using var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "JourneyOfFaith"
            });

            // BƯỚC 2: Tạo thư mục cha trên Google Drive để gom ảnh nhà thờ
            string googleFolderId = string.Empty;
            try
            {
                var folderMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = $"Church_{command.Name.RemoveVietnameseSigns().Replace(" ", "_")}_{Guid.NewGuid().ToString().Substring(0, 6)}",
                    MimeType = "application/vnd.google-apps.folder" ,// ĐÃ SỬA: Phải là kiểu folder chuẩn bài!

                    // LƯU Ý: Nếu muốn lưu vào thư mục cha cố định, hãy paste chuỗi ID thật của folder đó vào đây
                    Parents = new List<string> { parentFolderId }
                };

                var folderRequest = driveService.Files.Create(folderMetadata);
                folderRequest.Fields = "id";
                var folder = await folderRequest.ExecuteAsync(token);
                googleFolderId = folder.Id;
            }
            catch (Exception ex)
            {
                throw new System.Exception($"Tạo thư mục trên Google Drive thất bại: {ex.Message}");
            }

            // BƯỚC 3: Upload trực tiếp file từ memory lên Google Drive folder vừa tạo
            var uploadedImageIds = new List<string>();
            var uploadFiles = command.Files ?? [];

            foreach (var file in uploadFiles)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                    MimeType = file.ContentType,
                    Parents = new List<string> { googleFolderId }
                };

                using (var fileStream = file.OpenReadStream())
                {
                    var uploadRequest = driveService.Files.Create(fileMetadata, fileStream, file.ContentType);
                    uploadRequest.Fields = "id";

                    var uploadProcess = await uploadRequest.UploadAsync(token);
                    token.ThrowIfCancellationRequested();
                    if (uploadProcess.Status != Google.Apis.Upload.UploadStatus.Completed ||
                        string.IsNullOrWhiteSpace(uploadRequest.ResponseBody?.Id))
                    {
                        throw new InvalidOperationException(
                            $"Tải ảnh {file.FileName} lên Drive thất bại.", uploadProcess.Exception);
                    }

                    // Lưu ID ảnh Google trả về để ném vào CSDL
                    uploadedImageIds.Add(uploadRequest.ResponseBody.Id);
                }
            }

            // BƯỚC 4: Tạo Entity Church và gán bộ ID ảnh Google Drive vào
            var church = new Church(command.Name, command.Thumbnail ?? string.Empty,
                                    command.Website ?? string.Empty, command.Address ?? string.Empty, command.DioceseId,
                                    command.Latitude, command.Longitude, userId, userId, command.Boss ?? string.Empty,
                                    command.Description ?? string.Empty);

            // Gán trực tiếp ID ảnh từ Drive vào DB, sau này chỉ cần lôi ID ra tạo link src cho ảnh
            church.SetImages(uploadedImageIds
                .Select(driveId => new ChurchImage
                {
                    ImageName = driveId,
                    CreatedUser = userId,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList());
            await _unitOfWork.BeginTransactionAsync(token);
            try
            {
            await _repo.AddAsync(church, token);

            await _unitOfWork.CommitTransactionAsync(cancellationToken: token);
            return church.Id;
            } catch
            {
               await _unitOfWork.RollBackTransactionAsync(cancellationToken: token);
                throw;
            }
            // BƯỚC 5: Lưu thông tin vào DB thông qua Repository
        }
    }
}
