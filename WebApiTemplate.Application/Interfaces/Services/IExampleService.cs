using WebApiTemplate.Application.Dtos;
using WebApiTemplate.Domain.Common;

namespace WebApiTemplate.Application.Interfaces.Services
{
    /// <summary>
    /// Defines the contract for the example service, which provides CRUD operations and search functionality.
    /// </summary>
    public interface IExampleService
    {
        /// <summary>
        /// Searches for a collection of items based on the provided search request, with pagination support.
        /// </summary>
        /// <param name="request">The search request containing filter criteria, page number, and page size.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a paginated response with a list of <see cref="ExampleDto"/> items.
        /// </returns>
        Task<PaginatedResponse<ExampleDto>> SearchAsync(SearchRequest request);

        /// <summary>
        /// Retrieves an item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="ExampleDto"/> for the requested item.
        /// </returns>
        Task<ExampleDto> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new item based on the provided request data.
        /// </summary>
        /// <param name="request">The request data to create the new item.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="ExampleDto"/> for the newly created item.
        /// </returns>
        Task<ExampleDto> CreateAsync(ExampleDto? request);

        /// <summary>
        /// Updates an existing item based on the provided request data.
        /// </summary>
        /// <param name="request">The request data to update the item.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the updated <see cref="ExampleDto"/>.
        /// </returns>
        Task<ExampleDto> UpdateAsync(ExampleDto request);

        /// <summary>
        /// Deletes an item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item to be deleted.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean indicating whether the delete operation was successful.
        /// </returns>
        Task<bool> DeleteAsync(string id);
    }
}