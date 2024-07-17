using AutoMapper;
using Web.Application.Dtos;
using Web.Domain.Entities;

namespace Web.Application.Configurations.MappingProfiles.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BlogDto, Blog>().ReverseMap();
        }
    }
}