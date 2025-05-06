using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace WebApiTemplate.UnitTests.Common
{
    public static class TestHelper
    {
        public static IHttpContextAccessor FakeHttpContextAccessor(string userId = "default-id", string email = "default@example.com")
        {
            // Create a fake user with claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email)
            };

            var identity = new ClaimsIdentity(claims, "mock");
            var user = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext
            {
                User = user
            };

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);

            return httpContextAccessorMock.Object;
        }
    }
}