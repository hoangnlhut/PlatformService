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


        [HttpPost]
        public ActionResult Test()
        {
            Console.WriteLine("Command Service is up and running!");
            return Ok("Command Service is up and running!");
        }
    }
}
