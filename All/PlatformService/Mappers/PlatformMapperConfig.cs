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
        }
    }
}
