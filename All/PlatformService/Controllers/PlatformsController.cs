using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.Repository;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;
        private const string eventTypePlatformPublished = "Platform_Published";

        public PlatformsController(IPlatformRepository repository, ICommandDataClient commandDataClient, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _messageBusClient = messageBusClient;
            _mapper = mapper;
        }

        // GET: api/Platforms
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platforms = _repository.GetAll();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        // GET: api/Platforms/{id}
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = _repository.GetById(id);
            if (platform == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        // POST: api/Platforms
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.Create(platformModel);
            _repository.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            #region Call the Command Service to sync the new platform synchronously - NOT USING NOW
            //try
            //{
            //    await _commandDataClient.SendPlatformToCommand(platformReadDto);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Could not send SendPlatformToCommand: {ex.Message} - {ex.InnerException?.Message ?? "No inner exception"}");
            //}
            #endregion

            #region Send the new platform to the message bus RabbitMQ asynchronously 
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto); platformPublishedDto.Event = eventTypePlatformPublished;
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send PublishNewPlatform: {ex.Message} - {ex.InnerException?.Message ?? "No inner exception"}");
            }
            #endregion


            return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
        }
    }
}
