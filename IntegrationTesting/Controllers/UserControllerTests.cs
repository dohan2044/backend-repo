// using FluentAssertions;
// using Journey_of_faith.Application.usecases.users.commands;
// using System.Net;
// using System.Net.Http.Json;
// using System.Threading.Tasks;
// using Journey_of_faith.Api;
// using Xunit;

// namespace IntegrationTesting.Controllers
// {
//     public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
//     {
//         private readonly HttpClient _client;

//         public UserControllerTests(CustomWebApplicationFactory<Program> factory)
//         {
//             _client = factory.CreateClient();
//         }

//         [Fact]
//         public async Task CreateUser_ShouldReturnBadRequest_WhenUsernameOrEmailIsEmpty()
//         {
//             // Arrange
//             var command = new CreateUserCommand
//             {
//                 Username = "",
//                 Password = "Password123",
//                 Email = ""
//             };

//             // Act
//             var response = await _client.PostAsJsonAsync("/api/Users", command);

//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//         }

//         [Fact]
//         public async Task GetUsers_ShouldReturnSuccessStatusCode()
//         {
//             // Act
//             var response = await _client.GetAsync("/api/Users?page=1&pageSize=10");

//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//         }
//     }
// }
using System.Net;
using FluentAssertions;
using Journey_of_faith.Api;
using Xunit;

namespace IntegrationTesting.Controllers
{
    public class UserControllerTests
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UserControllerTests(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/Users?page=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}