using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TiendaPc.DLL.Services.Interfaces;

namespace TiendaPc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponenteController : ControllerBase
    {
        private readonly IComponenteService _service;
        public ComponenteController(IComponenteService service)
        {
            _service = service;
        }




    }
}
