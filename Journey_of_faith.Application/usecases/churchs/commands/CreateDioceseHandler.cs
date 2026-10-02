using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Application.exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.churchs;

namespace Journey_of_faith.Application.usecases.churchs.commands
{
    public class CreateDioceseHandler : IRequestHandler<CreateDioceseCommand, int>
    {
        private readonly IChurchRepository _churchRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChurchQueries _churchQueries;
        private readonly IUnitOfWork _unitOfWork;
        public CreateDioceseHandler(
            IChurchRepository churchRepository,
            ICurrentUserService currentUserService,
            IChurchQueries churchQueries,
            IUnitOfWork unitOfWork)
        {
            _churchRepository = churchRepository;
            _currentUserService = currentUserService;
            _churchQueries = churchQueries;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateDioceseCommand command, CancellationToken token)
        {
            if (await _churchQueries.DioceseNameExistsAsync(command.Name, token))
            {
                throw new UnprocessableEntityException("Tên Giáo xữ đã tòn tại");
            }
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                throw new UnauthorizationException("Người dùng không hợp lệ");
            }
            var diocese = new Diocese(command.Name, command.Website, command.Address ?? string.Empty, command.Thumbnail ?? string.Empty, userId);
            await _churchRepository.AddAsync(diocese, token);
            await _unitOfWork.SaveChangeAsync(token);
            return diocese.Id;
        }
    }
}
