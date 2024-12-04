using Mapster;

namespace Web.Application.Configurations.MappingProfiles.Mapster
{
    public class MappingRegistration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.Default.Settings.IgnoreNullValues = true;
            // Mapping from Entity to DTO.

            // Mapping from DTO to Entity
        }
    }
}