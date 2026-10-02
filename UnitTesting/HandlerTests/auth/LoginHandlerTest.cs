using FluentAssertions;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.usecases.auth.queries;
using Journey_of_faith.Application.exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTesting.HandlerTests.auth
{
    public class LoginHandlerTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly UserLoginHandler _handler;
        public LoginHandlerTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _handler = new UserLoginHandler(_authServiceMock.Object);
        }


        [Fact]
        public async Task Handle_ShouldReturnTokenAndRefreshToken_WhenCredentialsAreValid()
        {
            // Arrange 
            var query = new UserLoginQuery
            {
                Email = "validUser@example.com",
                Password = "validPassword"
            };

            var token = "mockedToken";
            var refreshToken = "mockedRefreshToken";
            int expiryDate = 60 * 60;

            _authServiceMock.Setup(s => s.Login(query.Email, query.Password))
                .ReturnsAsync(new LoginUserResponse(
                        true,
                        token,
                        refreshToken,
                        expiry: expiryDate
                    ));

            // Act

            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.success);
            Assert.Equal(token, result.token);
            Assert.Equal(refreshToken, result.refreshToken);

            Assert.Equal(expiryDate, result.expiry);

            _authServiceMock.Verify(s => s.Login(query.Email, query.Password), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserNotExists()
        {
            // arrange
            var query = new UserLoginQuery
            {
                Email = "invalidUser@example.com",
                Password = "invalidPassword"
            };

            _authServiceMock.Setup(e => e.Login(query.Email, query.Password))
                .ThrowsAsync(new NotFoundException("Tài khoản hoặc mật khẩu không chính xác"));

            // act 
            var result = await Assert.ThrowsAsync<NotFoundException>
                (() => _handler.Handle(query, CancellationToken.None));

            // assert
            Assert.NotNull(result);
            Assert.Equal("Tài khoản hoặc mật khẩu không chính xác", result.Message);

            _authServiceMock.Verify(e => e.Login(query.Email, query.Password), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenPasswordIsInCorrect()
        {
            // Arrange 
            var query = new UserLoginQuery
            {
                Email = "validUser@example.com",
                Password = "invalidPassword"
            };

            _authServiceMock.Setup(e => e.Login(query.Email, query.Password))
                .ThrowsAsync(new UnauthorizationException("Tài khoản hoặc mật khẩu không chính xác"));


            // Act 
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            var exception = await Assert.ThrowsAsync<UnauthorizationException>(act);

            Assert.NotNull(act);
            Assert.Equal("Tài khoản hoặc mật khẩu không chính xác", exception.Message);

            _authServiceMock.Verify(e => e.Login(query.Email, query.Password), Times.Once);

        }
    }
}
