using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.common.untils;
using Journey_of_faith.Application.usecases.churchs;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.entities.masslive;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Journey_of_faith.Application.usecases.churchs.commands;

public class UpdateChurchHandler : IRequestHandler<UpdateChurchCommand, int>
{
    private static readonly string[] Scopes = { DriveService.Scope.Drive };
    private readonly IChurchRepository _repo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration configuration;
    private readonly IChurchQueries _churchQueries;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateChurchHandler(
        IChurchRepository repo,
        ICurrentUserService currentUserService,
        IConfiguration configuration,
        IChurchQueries churchQueries,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _currentUserService = currentUserService;
        this.configuration = configuration;
        _churchQueries = churchQueries;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(UpdateChurchCommand command, CancellationToken token)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            throw new UnauthorizationException("Người dùng không hợp lệ");
        }
        if (!await _churchQueries.DioceseExistsAsync(command.DioceseId, token))
        {
            throw new NotFoundException("Không có giáo phận mà nhà thờ đăng ký.");
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

        string googleFolderId;
        try
        {
            var churchName = command.Name ?? string.Empty;
            var folderMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = $"Church_{churchName.RemoveVietnameseSigns().Replace(" ", "_")}_{Guid.NewGuid().ToString()[..6]}",
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { parentFolderId }
            };

            var folderRequest = driveService.Files.Create(folderMetadata);
            folderRequest.Fields = "id";
            var folder = await folderRequest.ExecuteAsync(token);
            googleFolderId = folder.Id;
        }
        catch (Exception ex)
        {
            throw new Exception($"Tạo thư mục trên Google Drive thất bại: {ex.Message}", ex);
        }

        var uploadedImageIds = new List<string>();
        var uploadFiles = command.Files ?? [];

        foreach (var file in uploadFiles)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                MimeType = file.ContentType,
                Parents = new List<string> { googleFolderId }
            };

            using var fileStream = file.OpenReadStream();
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

            uploadedImageIds.Add(uploadRequest.ResponseBody.Id);
        }

        var church = new Church(
            command.Id,
            command.Name ?? string.Empty,
            command.Email ?? string.Empty,
            command.Address ?? string.Empty,
            command.DioceseId,
            command.Boss ?? string.Empty,
            command.Description ?? string.Empty,
            userId
            );

        church.SetLocation(command.Latitude ?? 0, command.Longitude ?? 0);
        church.SetImages(uploadedImageIds
            .Select(driveId => new ChurchImage
            {
                ChurchId = command.Id,
                ImageName = driveId,
                CreatedUser = userId,
                CreatedAt = DateTime.UtcNow
            })
            .ToList());

        _repo.Update(church);
        await _unitOfWork.SaveChangeAsync(token);
        return church.Id;
    }
}
