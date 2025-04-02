namespace WebApiTemplate.Application.Dtos.Base
{
    /// <summary>
    /// Represents a base Data Transfer Object (DTO) that includes common properties for entities in the application.
    /// </summary>
    /// <remarks>
    /// This base DTO class provides properties that are commonly shared by entities in the system. It includes identifiers, language, tenant information,
    /// and timestamps for creation and modification. Derived DTOs can inherit from this class to extend its functionality while keeping the common fields.
    /// </remarks>
    public class BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        /// <value>The unique identifier.</value>
        /// <remarks>
        /// The ID is usually a string, which could represent a GUID or another unique value that identifies the entity.
        /// </remarks>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the language code for the entity. Defaults to "en".
        /// </summary>
        /// <value>The language code.</value>
        /// <remarks>
        /// The default language is set to "en", but this can be changed depending on the entity's language requirements.
        /// </remarks>
        public string Language { get; set; } = "en";

        /// <summary>
        /// Gets or sets the tenant identifier for multi-tenancy support.
        /// </summary>
        /// <value>The tenant identifier, or null if not applicable.</value>
        /// <remarks>
        /// This property is used for applications that support multi-tenancy. If the application is not multi-tenant, this field can be null.
        /// </remarks>
        public int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        /// <value>The creation date, or null if not specified.</value>
        /// <remarks>
        /// This timestamp indicates when the entity was first created in the system.
        /// </remarks>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user who created the entity.
        /// </summary>
        /// <value>The creator's identifier, or null if not available.</value>
        /// <remarks>
        /// This field stores the identifier of the user who created the entity.
        /// </remarks>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last modified.
        /// </summary>
        /// <value>The modification date, or null if not applicable.</value>
        /// <remarks>
        /// This timestamp indicates when the entity was last updated in the system.
        /// </remarks>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the entity.
        /// </summary>
        /// <value>The modifier's identifier, or null if not available.</value>
        /// <remarks>
        /// This field stores the identifier of the user who last modified the entity.
        /// </remarks>
        public string? ModifiedBy { get; set; }
    }
}