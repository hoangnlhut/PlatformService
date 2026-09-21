using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using CommandService.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId) {
            Console.WriteLine("--> Getting Commands for Platform: " + platformId);

            //check platform exists
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound("Platform not found");
            }

            var commands = _repository.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet]
        [Route("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId) {
            Console.WriteLine("--> Getting Command for Platform: " + platformId + ", Command: " + commandId);

            //check platform exists
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound("Platform not found");
            }

            var command = _repository.GetCommand(platformId, commandId);
            if (command == null) {
                return NotFound("Command not found");
            }
            var commandDto = _mapper.Map<CommandReadDto>(command);
            return Ok(commandDto);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandDto) {

            Console.WriteLine("--> Creating Command for Platform: " + platformId);
            //check platform exists
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound("Platform not found");
            }

            var command = _mapper.Map<Command>(commandDto);
            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var createdCommandDto = _mapper.Map<CommandReadDto>(command);
            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = createdCommandDto.Id }, createdCommandDto);
        }
    }
}
