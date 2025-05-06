using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using WebApiTemplate.Application.Interfaces.Repositories;

namespace WebApiTemplate.UnitTests.Common
{
    public class TestFixture : IDisposable
    {
        public Mock<IExampleRepository> ExampleRepositoryMock { get; }
        public Mock<IHttpContextAccessor> HttpContextAccessorMock { get; }
        public Mock<IMapper> MapperMock { get; }

        private bool _disposed; // Tracks whether the object has been disposed

        public TestFixture()
        {
            ExampleRepositoryMock = new Mock<IExampleRepository>();
            HttpContextAccessorMock = Mock.Get(TestHelper.FakeHttpContextAccessor("67f2ef2f-b25e-449c-9eb9-2bfb21fd7de6", "test@example.com"));
            MapperMock = new Mock<IMapper>();
            _disposed = false;
        }

        // Public Dispose method for deterministic cleanup
        public void Dispose()
        {
            Dispose(true); // Call the protected Dispose method with disposing = true
            GC.SuppressFinalize(this); // Suppress finalization since Dispose was called
        }

        // Protected virtual Dispose method to handle cleanup
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return; // Prevent multiple disposals
            }

            if (disposing)
            {
                ExampleRepositoryMock.Reset();
                HttpContextAccessorMock.Reset();
                MapperMock.Reset();
            }

            // Dispose unmanaged resources (if any)
            // This class doesn't have unmanaged resources, but this is where you'd clean them up.
            // Example: If you had a pointer to unmanaged memory, you'd free it here.

            _disposed = true; // Mark as disposed
        }

        // Finalizer (destructor) as a fallback if Dispose isn't called
        ~TestFixture()
        {
            Dispose(false); // Call Dispose with disposing = false to clean up unmanaged resources only
        }
    }
}