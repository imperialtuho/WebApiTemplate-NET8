using Asp.Versioning;
using WebApiTemplate.Application.Interfaces.Services;
using WebApiTemplate.Domain.Common;
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
        /// Retrieves example data for API version 1.0, demonstrating AES encryption and decryption.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing example data specific to API v1.0.
        /// The response follows HTTP status codes, returning <see cref="HttpStatusCode.OK"/> when successful.
        /// </returns>
        /// <remarks>
        /// This method serves as an example of AES encryption and decryption. It performs the following operations:
        /// <list type="bullet">
        /// <item><description>Encrypts a plaintext message using AES encryption.</description></item>
        /// <item><description>Validates the encrypted string to ensure it is a valid Base64 format.</description></item>
        /// <item><description>Decrypts the encrypted message back to its original plaintext.</description></item>
        /// <item><description>Verifies that the decrypted text matches the original plaintext.</description></item>
        /// </list>
        /// The final result is retrieved asynchronously from <see cref="IExampleService.GetByIdAsync"/> and returned in the response.
        /// </remarks>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsyncV1()
        {
            string password = "StrongPassword123";
            string plaintext = "Hello, world!";

            string encrypted = AesEncryptionHelper.Encrypt(plaintext, password);
            Console.WriteLine($"Encrypted: {encrypted}\n");
            Console.WriteLine($"Is encrypted string as base64 valid?: {CheckingHelper.IsBase64String(encrypted)}\n");

            string decrypted = AesEncryptionHelper.Decrypt(encrypted, password);

            Console.WriteLine($"Decrypted: {decrypted}\n");
            Console.WriteLine($"Is plaintext equals to descripted text?: {plaintext.Equals(decrypted)}");

            return Result(await exampleService.GetByIdAsync(string.Empty), HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves a paginated list of filtered example data for API version 2.0.
        /// </summary>
        /// <param name="searchRequest">
        /// The search request containing filtering criteria and pagination parameters.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the filtered and paginated example data.
        /// The response follows HTTP status codes, returning <see cref="HttpStatusCode.OK"/> when successful.
        /// </returns>
        /// <remarks>
        /// This endpoint processes search requests by applying dynamic filtering and pagination.
        /// The filtering logic is based on the <see cref="FilterCriteria"/> provided in the request body,
        /// allowing conditions such as equality, inequality, range comparisons, and string matching.
        /// The pagination parameters, <c>PageNumber</c> and <c>PageSize</c>, determine the subset of data returned.
        /// </remarks>
        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetAsyncV2([FromBody] SearchRequest searchRequest)
        {
            return Result(await exampleService.SearchAsync(searchRequest), HttpStatusCode.OK);
        }
    }
}