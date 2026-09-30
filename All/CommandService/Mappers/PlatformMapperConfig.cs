using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Mappers
{
    public class PlatformMapperConfig : Profile
    {
        public PlatformMapperConfig()
        {
            // source -> target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();

            CreateMap<PlatformPublishedDto, Platform>()
            // Map Id of PlatformPublishedDto to ExternalId of Platform explicitly
            .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Commands, opt => opt.Ignore());
        }
    }
}
