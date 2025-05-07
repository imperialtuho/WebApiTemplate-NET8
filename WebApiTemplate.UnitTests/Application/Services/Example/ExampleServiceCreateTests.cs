using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using WebApiTemplate.Application.Dtos;
using WebApiTemplate.Application.Services;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.UnitTests.Common;

namespace WebApiTemplate.UnitTests.Application.Services.Example
{
    [Collection("ExampleServiceTests")]
    public class ExampleServiceCreateTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;
        private readonly ExampleService _exampleService;
        private const string UserId = "67f2ef2f-b25e-449c-9eb9-2bfb21fd7de6";

        public ExampleServiceCreateTests(TestFixture fixture)
        {
            _fixture = fixture;
            // Define the UserId and other values for the test
            string userId = UserId; // Assuming UserId is defined in your test
            string email = "test@example.com";
            int tenantId = 123;
            var roles = new List<string> { "Admin", "User" };
            var permissions = new List<string> { "Read", "Write" };

            // Create a ClaimsPrincipal with the required claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Sid, userId)
            };

            // Add TenantId claim if needed
            if (tenantId != 0)
            {
                claims.Add(new Claim("tenant_id", tenantId.ToString())); // Replace "tenant_id" with the actual TenantIdClaim constant
            }

            // Add roles and permissions
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(permissions.Select(permission => new Claim("permission", permission))); // Replace "permission" with the actual Permission constant

            var identity = new ClaimsIdentity(claims, "mock"); // Ensure IsAuthenticated is true
            var user = new ClaimsPrincipal(identity);

            // Mock HttpContextAccessor to return the ClaimsPrincipal
            _fixture.HttpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);
            _exampleService = new ExampleService(
                _fixture.ExampleRepositoryMock.Object,
                _fixture.HttpContextAccessorMock.Object,
                _fixture.MapperMock.Object
            );
        }

        private void ResetMocks()
        {
            _fixture.ExampleRepositoryMock.Reset();
            _fixture.HttpContextAccessorMock.Reset();
            _fixture.MapperMock.Reset();
            // Reapply default HttpContext setup since it's set in the constructor
            _fixture.HttpContextAccessorMock.Setup(x => x.HttpContext!.User!.Identity!.Name).Returns(UserId);
        }

        [Fact]
        public async Task CreateAsync_ShouldUseUserIdFromLoginSession()
        {
            // Arrange
            ResetMocks(); // Resets all mocks, including HttpContextAccessorMock

            // Define the expected UserId
            string userId = "67f2ef2f-b25e-449c-9eb9-2bfb21fd7de6"; // Match the expected UserId
            string id = Guid.NewGuid().ToString();
            var exampleDto = new ExampleDto { Id = id };
            var exampleEntity = new ExampleEntity { Id = id, UserId = userId };
            var resultDto = new ExampleDto { Id = id };

            // Set up ClaimsPrincipal for GetUserSession
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, userId), // UserId
                new Claim(ClaimTypes.Name, "test@example.com"), // Email (required to avoid empty UserSession)
                // Optional: Add other claims if needed
                new Claim("tenant_id", "123"), // TenantId (adjust claim type if different)
                new Claim(ClaimTypes.Role, "Admin"), // Example role
                new Claim("permission", "Read") // Example permission (adjust claim type if different)
            };
            var identity = new ClaimsIdentity(claims, "mock"); // Ensure IsAuthenticated is true
            var user = new ClaimsPrincipal(identity);

            // Set up HttpContextAccessorMock after ResetMocks
            _fixture.HttpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(user);

            // Set up other mocks
            ExampleEntity? capturedEntity = null;

            _fixture.MapperMock
                .Setup(mapper => mapper.Map<ExampleEntity>(It.Is<ExampleDto>(dto => dto.Id == id)))
                .Returns(exampleEntity);
            _fixture.ExampleRepositoryMock
                .Setup(repo => repo.AddWithSaveChangesAndReturnModelAsync(It.IsAny<ExampleEntity>()))
                .Callback<ExampleEntity>(entity => capturedEntity = entity)
                .Returns(Task.FromResult(exampleEntity));
            _fixture.MapperMock
                .Setup(mapper => mapper.Map<ExampleDto>(It.Is<ExampleEntity>(e => e.Id == id)))
                .Returns(resultDto);

            // Act
            var result = await _exampleService.CreateAsync(exampleDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            _fixture.ExampleRepositoryMock.Verify(repo => repo.AddWithSaveChangesAndReturnModelAsync(It.IsAny<ExampleEntity>()), Times.Once());
            capturedEntity.Should().NotBeNull();
            capturedEntity!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleNullInput()
        {
            // Arrange
            ResetMocks();
            ExampleDto? exampleDto = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _exampleService.CreateAsync(exampleDto));
            _fixture.ExampleRepositoryMock.Verify(repo => repo.AddWithSaveChangesAndReturnModelAsync(It.IsAny<ExampleEntity>()), Times.Never());
            _fixture.MapperMock.Verify(mapper => mapper.Map<ExampleEntity?>(It.IsAny<ExampleDto>()), Times.Never());
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            ResetMocks();
            string id = Guid.NewGuid().ToString();
            var exampleDto = new ExampleDto { Id = id };
            var exampleEntity = new ExampleEntity { Id = id, UserId = UserId };
            var exception = new Exception("Repository failure");

            _fixture.MapperMock
                .Setup(mapper => mapper.Map<ExampleEntity>(It.Is<ExampleDto>(dto => dto.Id == id)))
                .Returns(exampleEntity);
            _fixture.ExampleRepositoryMock
                .Setup(repo => repo.AddWithSaveChangesAndReturnModelAsync(It.IsAny<ExampleEntity>()))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _exampleService.CreateAsync(exampleDto));
            ex.Message.Should().Be("Repository failure");
            _fixture.MapperMock.Verify(mapper => mapper.Map<ExampleDto>(It.IsAny<ExampleEntity>()), Times.Never());
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleNullHttpContext()
        {
            // Arrange
            ResetMocks();
            string id = Guid.NewGuid().ToString();
            var exampleDto = new ExampleDto { Id = id };
            var exampleEntity = new ExampleEntity { Id = id }; // UserId should be null/empty
            var resultDto = new ExampleDto { Id = id };

            var nullHttpContextMock = new Mock<IHttpContextAccessor>();
            nullHttpContextMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            var serviceWithNullContext = new ExampleService(
                _fixture.ExampleRepositoryMock.Object,
                nullHttpContextMock.Object,
                _fixture.MapperMock.Object
            );

            _fixture.MapperMock
                .Setup(mapper => mapper.Map<ExampleEntity>(It.Is<ExampleDto>(dto => dto.Id == id)))
                .Returns(exampleEntity);
            _fixture.ExampleRepositoryMock
                .Setup(repo => repo.AddWithSaveChangesAndReturnModelAsync(It.Is<ExampleEntity>(e => e == null || string.IsNullOrEmpty(e.UserId))))
                .Returns(Task.FromResult(exampleEntity));
            _fixture.MapperMock
                .Setup(mapper => mapper.Map<ExampleDto>(It.Is<ExampleEntity>(e => e.Id == id)))
                .Returns(resultDto);

            // Act
            var result = await serviceWithNullContext.CreateAsync(exampleDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            _fixture.ExampleRepositoryMock.Verify(repo => repo.AddWithSaveChangesAndReturnModelAsync(It.Is<ExampleEntity>(e => e == null || string.IsNullOrEmpty(e.UserId))), Times.Once());
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleMappingFailure()
        {
            // Arrange
            ResetMocks();
            string id = Guid.NewGuid().ToString();
            var exampleDto = new ExampleDto { Id = id };

            _fixture.MapperMock
                .Setup(mapper => mapper.Map<ExampleEntity>(It.Is<ExampleDto>(dto => dto.Id == id)))
                .Throws<InvalidOperationException>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _exampleService.CreateAsync(exampleDto));
            _fixture.ExampleRepositoryMock.Verify(repo => repo.AddWithSaveChangesAndReturnModelAsync(It.IsAny<ExampleEntity>()), Times.Never());
            _fixture.MapperMock.Verify(mapper => mapper.Map<ExampleDto>(It.IsAny<ExampleEntity>()), Times.Never());
        }
    }
}