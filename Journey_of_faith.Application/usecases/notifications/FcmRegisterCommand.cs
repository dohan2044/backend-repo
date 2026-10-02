using System.Net;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Journey_of_faith.Application.usecases.notifications;

public class FcmRegisterCommand : IRequest<Unit>
{
    public string Token {get; set;} = string.Empty;
    public string Platform {get; set;} = string.Empty;

}


public class FcmRegisterHandler : IRequestHandler<FcmRegisterCommand, Unit>
{
    private readonly IFirebaseNotification firebaseNotification;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService currentUserService;
    public FcmRegisterHandler(
        IFirebaseNotification firebaseNotification,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService
    )
    {
        this.firebaseNotification = firebaseNotification;
        this._unitOfWork = unitOfWork;
        this.currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(FcmRegisterCommand command, CancellationToken cancellationToken = default)
    {
            if(!Guid.TryParse(currentUserService.UserId, out Guid userId)) 
                throw new UnauthorizationException("Thông tin Người dùng không hợp lệ");

            var deviceToken = new DeviceToken
            {
                UserId = userId,
                Platform = command.Platform,
                Token = command.Token,
            };
            if(await firebaseNotification.DeviceTokenExistsAsync(deviceToken, cancellationToken))
                return Unit.Value;
            deviceToken.CreatedAt = DateTime.Now;
            await firebaseNotification.FcmRegisterAsync(deviceToken);
            await _unitOfWork.SaveChangeAsync();
            return Unit.Value;
                
    }
}