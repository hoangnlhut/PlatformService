using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using CommandService.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting GetPlatforms from CommandService");
            var platformItems = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }


        [HttpPost]
        public ActionResult CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platform = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platform);
            _repository.SaveChanges();
            return Ok("Platform created successfully!");
        }
    }
}
