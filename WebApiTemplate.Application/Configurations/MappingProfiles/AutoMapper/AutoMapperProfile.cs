using AutoMapper;
using WebApiTemplate.Application.Dtos;
using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Application.Configurations.MappingProfiles.AutoMapper
{
    /// <summary>
    /// Configures the mappings for AutoMapper, defining how domain entities map to DTOs and vice versa.
    /// </summary>
    /// <remarks>
    /// This class inherits from <see cref="Profile"/> and defines the mapping configurations between domain
    /// entities and their corresponding data transfer objects (DTOs). The mappings can be used by AutoMapper to
    /// automatically convert between the two types. Additionally, the mappings can be customized as needed, including
    /// using specific member mappings, value conversions, or reverse mappings.
    /// </remarks>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class and defines mapping configurations.
        /// </summary>
        public AutoMapperProfile()
        {
            // Define a simple mapping between ExampleEntity and ExampleDto, with reference preservation and reverse mapping.
            CreateMap<ExampleEntity, ExampleDto>()
                .PreserveReferences()  // Ensures that object references are preserved during mapping, useful for cyclic references.
                .ReverseMap();  // Adds a reverse mapping so that ExampleDto can be mapped back to ExampleEntity.

            /* Example of mapping profile
            CreateMap<Post, PostDto>()
                .ForMember(p => p.MinutesToRead, opt => opt.MapFrom(src => ReadingTimeEstimatorHelper.EstimateMinutesToRead(src.Content)))
                .ForMember(p => p.Slug, opt => opt.MapFrom(src => StringHelper.ToSlug(src.Title)))
                .ReverseMap();
            */
        }
    }
}