using AutoMapper;

namespace WebApiTemplate.Application.Configurations.MappingProfiles.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            /* Example of mapping profile
            CreateMap<Post, PostDto>()
                .ForMember(p => p.MinutesToRead, opt => opt.MapFrom(src => ReadingTimeEstimatorHelper.EstimateMinutesToRead(src.Content)))
                .ForMember(p => p.Slug, opt => opt.MapFrom(src => StringHelper.ToSlug(src.Title)))
                .ReverseMap();
            */
        }
    }
}