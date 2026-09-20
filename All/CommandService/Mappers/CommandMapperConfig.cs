using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Mappers
{
    public class CommandMapperConfig : Profile
    {
        public CommandMapperConfig()
        {
            // source -> target
            CreateMap<Command, CommandReadDto>();

            CreateMap<CommandCreateDto, Command>();
        }
    }
}
