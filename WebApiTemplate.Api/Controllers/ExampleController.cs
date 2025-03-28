using Asp.Versioning;
using WebApiTemplate.Application.Interfaces.Services;
using WebApiTemplate.Domain.Helpers;

namespace WebApiTemplate.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ExampleController(IExampleService exampleService) : BaseController
    {
        /// <summary>
        /// API version 1.
        /// </summary>
        /// <returns>Data v1.</returns>
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
        /// API version 2.
        /// </summary>
        /// <returns>Data v2.</returns>
        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetV2()
        {
            return Result(exampleService.GetByIdAsync(string.Empty), HttpStatusCode.OK);
        }
    }
}