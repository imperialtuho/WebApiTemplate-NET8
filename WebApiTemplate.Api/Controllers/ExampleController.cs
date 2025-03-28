using Asp.Versioning;
using WebApiTemplate.Application.Interfaces.Services;
using WebApiTemplate.Domain.Helpers;

namespace WebApiTemplate.Api.Controllers
{
    /// <summary>
    /// Example API controller supporting multiple API versions.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for versioned API responses.
    /// </remarks>
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ExampleController(IExampleService exampleService) : BaseController
    {
        /// <summary>
        /// Retrieves example data for API version 1.0.
        /// </summary>
        /// <returns>
        /// Returns example data specific to API v1.0.
        /// </returns>
        /// <remarks>
        /// This method demonstrates AES encryption and decryption before returning the response.
        /// </remarks>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        public IActionResult GetV1()
        {
            string password = "StrongPassword123";
            string plaintext = "Hello, world!";

            string encrypted = AesEncryptionHelper.Encrypt(plaintext, password);
            Console.WriteLine($"Encrypted: {encrypted}");

            string decrypted = AesEncryptionHelper.Decrypt(encrypted, password);
            Console.WriteLine($"Decrypted: {decrypted}");

            Console.WriteLine($"Is plaintext equals to descripted text?: {plaintext.Equals(decrypted)}");

            return Result(exampleService.GetByIdAsync(string.Empty), HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves example data for API version 2.0.
        /// </summary>
        /// <returns>
        /// Returns example data specific to API v2.0.
        /// </returns>
        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetV2()
        {
            return Result(exampleService.GetByIdAsync(string.Empty), HttpStatusCode.OK);
        }
    }
}