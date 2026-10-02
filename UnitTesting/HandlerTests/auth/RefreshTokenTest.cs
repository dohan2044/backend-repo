using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.auth.commands;
using Journey_of_faith.Application.exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.HandlerTests.auth
{
    public class RefreshTokenTest
    {
        private readonly Mock<IAuthService> _mock;
        private readonly CreateRefreshTokenHandler _handler;

        public RefreshTokenTest()
        {
            _mock = new Mock<IAuthService>();
            _handler = new CreateRefreshTokenHandler(_mock.Object);
        }


        [Fact]
        public async Task Handle_ShouldReturnNewTokenAndRefreshToken_WhenRefreshTokenValid()
        {
            // arrange
            var command = new CreateRefreshTokenCommand
            {
                Token = "kjsdfkdjfksđkfjdf"
            };
            var token = "NewToken";
            var refreshToken = "NewRefreshToken";
            int expiry = 3600;

            _mock.Setup(x => x.RefreshToken(command.Token))
                .ReturnsAsync(new LoginUserResponse(
                    status: true,
                    token: token,
                    refreshToken: refreshToken,
                    expiry: expiry
                    ));
            // act 
            var result = await _handler.Handle(command, CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.True(result.status);
            Assert.Equal(token, result.token);
            Assert.Equal(refreshToken, result.refreshToken);

            _mock.Verify(x => x.RefreshToken(command.Token), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRefreshTokenInValid()
        {
            // arrange
            var command = new CreateRefreshTokenCommand
            {
                Token = "invalidToken"
            };

            _mock.Setup(x => x.RefreshToken(command.Token))
                .ThrowsAsync(new UnauthorizationException("Refresh token đã hết hạn hoặc không hợp lệ."));

            // act & assert
            await Assert.ThrowsAsync<UnauthorizationException>
                (() => _handler.Handle(command, CancellationToken.None));

            _mock.Verify(e => e.RefreshToken(command.Token), times: Times.Once);
        }
    }
}
