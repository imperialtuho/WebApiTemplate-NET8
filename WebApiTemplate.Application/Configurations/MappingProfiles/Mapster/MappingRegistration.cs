using Mapster;

namespace WebApiTemplate.Application.Configurations.MappingProfiles.Mapster
{
    /// <summary>
    /// Configures the mapping between entities and DTOs using the Mapster library.
    /// </summary>
    /// <remarks>
    /// The <see cref="MappingRegistration"/> class is responsible for setting up the mapping configuration
    /// between entities and their corresponding data transfer objects (DTOs). It defines how to map properties
    /// between objects, including how to ignore null values and other settings for the mapping behavior.
    /// </remarks>
    public class MappingRegistration : IRegister
    {
        /// <summary>
        /// Registers the mapping configuration between entities and DTOs.
        /// </summary>
        /// <param name="config">The <see cref="TypeAdapterConfig"/> instance used to configure mappings.</param>
        /// <remarks>
        /// This method sets up default mapping settings and defines specific mappings between entities and DTOs.
        /// It is intended to be called during the application startup to ensure that Mapster mappings are properly
        /// registered and configured.
        /// </remarks>
        public void Register(TypeAdapterConfig config)
        {
            // Set default mapping settings to ignore null values.
            config.Default.Settings.IgnoreNullValues = true;

            // Example: Mapping from Entity to DTO (you can define specific mappings here)
            // config.NewConfig<EntityType, DtoType>();

            // Example: Mapping from DTO to Entity (you can define specific mappings here)
            // config.NewConfig<DtoType, EntityType>();
        }
    }
}