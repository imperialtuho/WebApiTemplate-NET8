using WebApiTemplate.Application.Dtos;

namespace WebApiTemplate.Application.Interfaces.ExternalProviders
{
    /// <summary>
    /// Defines a contract for interacting with an external identity API, providing methods for retrieving user information.
    /// </summary>
    /// <remarks>
    /// This interface serves as a bridge between the application and an external identity provider. It defines methods
    /// for fetching user details based on a single identifier or a list of identifiers. The methods return user data
    /// in the form of <see cref="UserDto"/> objects, allowing the application to interact with the identity provider
    /// and retrieve the necessary user information.
    /// </remarks>
    public interface IIdentityApi
    {
        /// <summary>
        /// Asynchronously retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="UserDto"/> corresponding to the user, or null if not found.</returns>
        /// <remarks>
        /// This method sends a request to the external identity API to retrieve user information based on the provided
        /// user ID. If the user is found, the result contains the corresponding <see cref="UserDto"/> object.
        /// If the user is not found, the result will be null.
        /// </remarks>
        Task<UserDto?> GetUserByIdAsync(string id);

        /// <summary>
        /// Asynchronously retrieves a list of users by their unique identifiers.
        /// </summary>
        /// <param name="ids">A list of unique identifiers for the users.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of <see cref="UserDto"/> objects corresponding to the users, or null if none are found.</returns>
        /// <remarks>
        /// This method sends a request to the external identity API to retrieve user information for multiple users.
        /// It accepts a list of user IDs and returns a list of <see cref="UserDto"/> objects for the users found.
        /// If no users are found, the result will be null or an empty list.
        /// </remarks>
        Task<IList<UserDto>?> GetUserByIdsAsync(IList<string> ids);
    }
}