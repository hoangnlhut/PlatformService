using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Mappers
{
    public class PlatformMapperConfig : Profile
    {
        public PlatformMapperConfig()
        {
            // source -> target
            CreateMap<Platform, PlatformReadDto>();

            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();

            CreateMap<Platform, GrpcPlatformModel>()
                .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));
        }
    }
}
