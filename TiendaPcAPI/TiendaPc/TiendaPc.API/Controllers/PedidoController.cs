﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                var pedido = await _pedidoService.GetById(id);
                if (pedido == null)
                    return NotFound(new {message = "no se encontro tu pedido" });
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
            }
        }
        [HttpGet("GetAll-DetallesPedido")]
        public async Task<IActionResult> GetPedidosFiltros(int idPedido)
        {
            try
            {
                if (idPedido == 0)
                {
                    return BadRequest(new { messaage = "Debe proporcionar la id del pedido para encontrar sus detalles" });
                }
                var DetallesPedido = await _pedidoService.GetAllDetallesPedido(idPedido); 
                if (DetallesPedido.Count == 0)
                {
                    return NotFound(new { messaage = "No se encontraron detalles para pedido proporcionados" });
                }
                return Ok(DetallesPedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
            }
        }

        [HttpGet("Get-PedidoDto")]
        public async Task<IActionResult> GetByIdPedidoDto([FromQuery] int id)
        {
            try
            {
                return Ok(await _pedidoService.GetByIdPedidoDto(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
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
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
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
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
            }
        }



        [HttpPost("Nuevo-Pedido")]
        public async Task<IActionResult> PostPedido([FromBody] Pedido pedido)
        {
            try
            {
                if(pedido == null)
                {
                    return BadRequest(new { message = "El pedido esta vacío." });
                }
                if(!await _pedidoService.Save(pedido))
                {
                    return BadRequest(new { message = "El pedido no fue se pudo registrar" });
                }
                int idCliente = pedido.IdCliente;
                decimal total = pedido.DetallesPedidos.Sum(detalle => detalle.Cantidad * detalle.PrecioUnitario * (1 - (detalle.Descuento ?? 0) / 100));
                int cantidad = pedido.DetallesPedidos.Sum(d => d.Cantidad);
                if(pedido.DescGamerCoins > 0)
                    await _pedidoService.DescontarGamerCoins(idCliente);

                await _pedidoService.AsignarGamerCoins(idCliente, total, cantidad);

                return Ok(new { message = "El pedido se registro con exito" });
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 50000)
            {
                // Captura el error específico lanzado por el trigger
                return BadRequest(new { message = sqlEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al registrar el pedido" + ex.ToString()});
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


        [HttpPatch("LowOrder-Pedido")]
        public async Task<IActionResult> BajaPedido(int id, string motivoCancelacion)
        {
            try
            {
                if (!await _pedidoService.LowOrder(id, motivoCancelacion))
                {
                    return BadRequest(new { message = "El pedido ya se encuentra cancelado, o no existe."});
                }
                return Ok(new {message = "El pedido fue cancelado con exito"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
            }
        }

        [HttpPatch("ConfirmOrder-Pedido")]
        public async Task<IActionResult> ConfrimrOrder(int id)
        {
            try
            {
                if (!await _pedidoService.ConfrimrOrder(id))
                {
                    return BadRequest(new { message = "El pedido no esta pendiente, o no existe." });
                }
                return Ok(new { message = "El pedido fue cancelado con exito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno: " + ex.ToString() });
            }
        }

    }
}
