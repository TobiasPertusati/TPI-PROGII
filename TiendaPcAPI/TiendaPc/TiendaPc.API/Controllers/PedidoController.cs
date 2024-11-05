using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TiendaPc.DLL.Models;
using TiendaPc.DLL.Services.Interfaces;

namespace TiendaPc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }


        [HttpGet("get-all-pedidos")]
        public async Task<IActionResult> GetAllPedidos()
        {
            try
            {
                var pedidos = await _pedidoService.GetAll();
                if (pedidos == null)
                {
                    return NotFound();
                }
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno : " + ex);
            }
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            try
            {
                return Ok(await _pedidoService.GetById(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
        }

        [HttpGet("GetAll-Pedidos-Fitros")]
        public async Task<IActionResult> GetPedidosFiltros(DateTime? fechaDesde, DateTime? fechaFin, int? idFormaPago, string? estado)
        {
            try
            {
                if(idFormaPago == 0)
                {
                    idFormaPago = null;
                }
                var pedidos = await _pedidoService.GetPedidosFiltros(fechaDesde, fechaFin, idFormaPago, estado);
                if (pedidos.Count == 0)
                {
                    return NotFound(new { messaage = "No se encontraron pedidos con los valores proporcionados" });
                }
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());

            }
        }

        [HttpGet("get-pedidos-pcArmadas")]
        public async Task<IActionResult> GetPedidosPCArmadas()
        {
            try
            {
                return Ok(await _pedidoService.GetPedidosPCArmadas());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());

            }
        }



        [HttpPost("insertar-pedido")]
        public async Task<IActionResult> PostPedido([FromBody] Pedido pedido)
        {
            try
            {
                if(pedido == null)
                {
                    return BadRequest("El pedido esta vacío.");
                }
                return Ok(await _pedidoService.Save(pedido));
            }
            catch (SqlException ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
            catch (Exception)
            {
                return StatusCode(500,"Error interno al registrar el pedido");
            }
        }

        //[HttpPut("actualizar-pedido/{id}")]
        //public async Task<IActionResult> PutPedido(int id, [FromBody] Pedido pedido)
        //{
        //    try
        //    {
        //        if (pedido == null)
        //        {
        //            return BadRequest("El pedido esta vacío.");
        //        }
        //        return Ok(await _pedidoService.Update(id, pedido));
        //    }
        //    catch (SqlException ex)
        //    {
        //        return StatusCode(500, "error interno: " + ex.ToString());
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500,"Error interno al actualizar el pedido");
        //    }
        //}


        [HttpPatch("bajaLogica-pedido/{id}/{motivoCancelacion}")]
        public async Task<IActionResult> BajaPedido(int id, string motivoCancelacion)
        {
            try
            {
                return Ok(await _pedidoService.LowOrder(id,motivoCancelacion));
            }
            catch (SqlException ex)
            {
                return StatusCode(500, "error interno: " + ex.ToString());
            }
            catch (Exception)
            {
                return StatusCode(500,"Error al dar de baja el pedido");
            }
        }

    }
}
