using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Text.RegularExpressions;
using TiendaPc.DLL.Models;
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

        [HttpGet("GetAll-Componentes")]
        public async Task<IActionResult> GetAllAsync() 
        {
            try
            {
                // trae todos los componentes activos.
                List<Componente> componentes = await _service.GetAllAsync();
                if (componentes.Count == 0)
                    return NotFound(new { message = "No se encontraron componentes"});

                return Ok(componentes);
            }
            catch (Exception ex)
            {
                return StatusCode(500,"error interno: " + ex.ToString());
            }

        }
        [HttpGet("GetAllFiltro-Componentes")]
        public async Task<IActionResult> FiltrarComponentes([FromQuery] bool? estado, [FromQuery] int? tipoComponenteId, [FromQuery] int? marcaId)
        {
            try
            {
                if (tipoComponenteId == 0)
                {
                    tipoComponenteId = null;
                }
                if (marcaId == 0)
                {
                    marcaId = null;
                }
                List<Componente> componentes = await _service.GetAllFiltrosAsync(marcaId,tipoComponenteId,estado);
                if (componentes.Count == 0)
                {
                    return NotFound(new { message = "No se encontraron componentes"});
                }
                return Ok(componentes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
            }
        }

        [HttpGet("GetById-Componente")]
        public async Task<IActionResult> GetByIdAsync([FromQuery] int id)
        {
            try
            {
                Componente componente = await _service.GetByIdAsync(id);
                if (componente == null)
                    return NotFound(new { message = "No se encontro un componente con esa id o no existe." });

                return Ok(componente);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });

            }
        }

        [HttpPost("Create-Componente")]
        public async Task<IActionResult> CreateAsync([FromBody] Componente componente)
        {
            try
            {
                bool res = await _service.CreateAsycn(componente);
                if (!res)
                    return BadRequest(new { message = "No se pudo crear el componente" });

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }

        [HttpPut("Update-Componente")]
        public async Task<IActionResult> UpdateAsync([FromBody] Componente componente)
        {
            try
            {
                bool res = await _service.UpadateAsycn(componente);
                if (!res)
                    return BadRequest(new { message = "No se pudo modificar el componente" });

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }

        [HttpPut("Low-Componente")]
        public async Task<IActionResult> LowComponenteAsync(int id)
        {
            try
            {
                bool res = await _service.LowComponenteAsycn(id);
                if (!res)
                    return BadRequest(new { message = "No se pudo dar de baja el componente"});

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }

        // CONTROLERS DE TESTEO IGNORAR

        [HttpPost("nueva-spec")]
        public async Task<IActionResult> nuevaSpec([FromBody] Especificacion spec)
        {
            try
            {
                bool res = await _service.nuevaEspecificacion(spec);
                if (!res)
                    return BadRequest(new { message = "No se pudo crear la nueva especificación" });

                return Ok(new { message = "Especificación cargada con existo" });

            }
            catch (Exception ex)
            {
                // Buscar el id_spec en el mensaje de error
                var regex = new Regex(@"ID de la especificación existente: (\d+)");
                var match = regex.Match(ex.Message);

                if (match.Success)
                {
                    // Extraer el id_spec si existe en el mensaje
                    int idSpecExistente = int.Parse(match.Groups[1].Value);
                    return BadRequest(new
                    {
                        message = "Ya existe una especificación con ese nombre",
                        id_spec = idSpecExistente
                    });
                }
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }
        
        [HttpGet("GetEspecificacionById")]
        public async Task<IActionResult> GetEspecificacionById([FromQuery] int idEspecificacion)
        {
            try
            {
                var spec = await _service.GetEspecificacionById(idEspecificacion);
                if (spec == null)
                    return BadRequest(new { message = "No se encontro la especificación" });

                return Ok(spec);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }
        [HttpGet("GetAll-TipoEspecificacion")]

        public async Task<IActionResult> GetAllTipoEspecificacion([FromQuery] int idComponente)
        {
            try
            {
                var lst = await _service.GetAllTipoEspecificacion(idComponente);
                if (lst.Count == 0)
                    return BadRequest(new { message = "No se encontraron especificación para ese tipo de componente" });

                return Ok(lst);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        } 
        [HttpGet("GetAll-EspecificacionesByIdComp")]

        public async Task<IActionResult> GetAllEspecificacionesByIdComp([FromQuery] int idComponente)
        {
            try
            {
                var lst = await _service.GetAllEspecificacionesByIdComp(idComponente);
                if (lst.Count == 0)
                    return BadRequest(new { message = "No se encontraron especificación para ese componente" });

                return Ok(lst);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }

        [HttpPost("nueva-EspecificacionComponente")]
        public async Task<IActionResult> nuevaEspecificacionComponente([FromBody] EspecificacionComponente specComp)
        {
            try
            {
                bool res = await _service.nuevaEspecificacionComponente(specComp);
                if (!res)
                    return BadRequest(new { message = "No se pudo crear la nueva especificación" });

                return Ok(new { message = "Especificación cargada con existo" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }
    }
}
