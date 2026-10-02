// using FluentAssertions;
// using Journey_of_faith.Application.usecases.auth.commands;
// using Journey_of_faith.Application.usecases.auth.queries;
// using System.Net;
// using System.Net.Http.Json;
// using System.Threading.Tasks;
// using Journey_of_faith.Api;
// using Xunit;

// namespace IntegrationTesting.Controllers
// {
//     public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
//     {
//         private readonly HttpClient _client;
//         private readonly CustomWebApplicationFactory<Program> _factory;

//         public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
//         {
//             _factory = factory;
//             _client = factory.CreateClient();
//         }

//         [Fact]
//         public async Task Login_ShouldReturnBadRequest_WhenEmailIsEmpty()
//         {
//             // Arrange
//             var query = new UserLoginQuery
//             {
//                 Email = "",
//                 Password = "Password123"
//             };

//             // Act
//             var response = await _client.PostAsJsonAsync("/api/Auth/login", query);

//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//         }

//         [Fact]
//         public async Task AddPermission_ShouldReturnBadRequest_WhenRoleNameIsEmpty()
//         {
//             // Arrange
//             var command = new AddPermissionCommand
//             {
//                 RoleName = "",
//                 Permissions = new System.Collections.Generic.List<string> { "Read" }
//             };

//             // Act
//             var response = await _client.PostAsJsonAsync("/api/Auth/roles/add-permission", command);

//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//         }
//     }
// }
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Journey_of_faith.Api;
using Xunit;

namespace IntegrationTesting.Controllers
{
    public class AuthControllerTests
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsValid()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "Admin@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsInvalid()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "invalid",
                Password = "wrong"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}