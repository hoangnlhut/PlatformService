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
        }
    }
}
