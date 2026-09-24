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
            .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
        }
    }
}
