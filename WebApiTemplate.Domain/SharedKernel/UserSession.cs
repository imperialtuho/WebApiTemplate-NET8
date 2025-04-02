namespace WebApiTemplate.Domain.SharedKernel
{
    /// <summary>
    /// Represents a user's session details, including user-specific information such as user ID, roles, permissions, tenant ID, and email.
    /// </summary>
    /// <remarks>
    /// This class is used to store the current authenticated user's session data, including their roles, permissions,
    /// and tenant context. It helps in managing the user's identity and access control during the session lifecycle.
    /// </remarks>
    public sealed class UserSession
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        /// <value>The user's unique identifier (usually a GUID or string).</value>
        /// <remarks>
        /// The UserId is typically used to reference the user across the system. It should uniquely identify the user
        /// across different services.
        /// </remarks>
        public string UserId { set; get; }

        /// <summary>
        /// Gets or sets the list of roles associated with the user.
        /// </summary>
        /// <value>A list of role names that the user belongs to.</value>
        /// <remarks>
        /// Roles are used to manage user permissions and access control within the application. Each role represents
        /// a specific set of privileges that the user possesses.
        /// </remarks>
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Gets or sets the list of permissions assigned to the user.
        /// </summary>
        /// <value>A list of permission names associated with the user.</value>
        /// <remarks>
        /// Permissions define specific actions that the user is allowed to perform within the application.
        /// They can be granular and used to control access at a more detailed level than roles alone.
        /// </remarks>
        public List<string>? Permissions { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the tenant to which the user belongs.
        /// </summary>
        /// <value>The tenant identifier, or null if the user is not associated with any tenant.</value>
        /// <remarks>
        /// TenantId is used in multi-tenant applications to associate users with a specific tenant context.
        /// If null, the user is assumed to not belong to any tenant.
        /// </remarks>
        public int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        /// <value>The user's email address.</value>
        /// <remarks>
        /// The email address is typically used for user authentication and communication purposes.
        /// It can also be used for identity verification or notifications.
        /// </remarks>
        public string? Email { get; set; }
    }
}