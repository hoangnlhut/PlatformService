using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        //private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public PlatformsController( IMapper mapper)
        {
            //_repository = repository;
            _mapper = mapper;
        }

        //// GET: api/Platforms
        //[HttpGet]
        //public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        //{
        //    var platforms = _repository.GetAll();
        //    return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        //}

        [HttpPost]
        [Route("test")]
        public ActionResult Test()
        {
            Console.WriteLine("Command Service is up and running!");
            return Ok("Command Service is up and running!");
        }
    }
}
