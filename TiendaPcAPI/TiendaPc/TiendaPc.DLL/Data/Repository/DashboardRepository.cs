using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaPc.API.DTO;
using TiendaPc.DLL.Data.Repository.Interfaces;
using TiendaPc.DLL.DTO;
using TiendaPc.DLL.Models;
using TiendaPc.DLL.Models.DTO;

namespace TiendaPc.DLL.Data.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DB_TIENDA_PCContext _context;

        public DashboardRepository(DB_TIENDA_PCContext context)
        {
            _context = context;
        }

        public async Task<List<DTOVentasMarcas>> GetEstadisticasMarcas()
        {
            var query = from marca in _context.Marcas
                        join componente in _context.Componentes on marca.IdMarca equals componente.IdMarca
                        join detallePedido in _context.DetallesPedidos on componente.IdComponente equals detallePedido.IdComponente
                        group detallePedido by marca.NombreMarca into g
                        select new DTOVentasMarcas
                        {
                            Marcas = g.Key, 
                            TotalVentas = g.Count(), 
                            ImporteTotal = g.Sum(dp => dp.Cantidad * dp.PrecioUnitario),  
                            PromedioVenta = g.Average(dp => dp.Cantidad * dp.PrecioUnitario)  
                        };

            var orderedQuery = query.OrderByDescending(x => x.TotalVentas).Take(5);

            List<DTOVentasMarcas> result = await orderedQuery.ToListAsync();
            return result;
        }

        public async Task<List<DTOEstadisticasTipoComponente>> GetDTOEstadisticasTipoComponentes()
        {
            var query = from tipoComponente in _context.TiposComponentes
                        join componente in _context.Componentes on tipoComponente.IdTipoComponente equals componente.IdTipoComponente
                        join detallePedido in _context.DetallesPedidos on componente.IdComponente equals detallePedido.IdComponente
                        group detallePedido by tipoComponente.Tipo into g
                        select new DTOEstadisticasTipoComponente
                        {
                            TipoComponente = g.Key,
                            VentasDelTipo = g.Count(),
                            ImporteTotal = g.Sum(dp => dp.Cantidad * dp.PrecioUnitario),
                            PromedioVenta = g.Average(dp => dp.Cantidad * dp.PrecioUnitario)
                        };

            var orderedQuery = query.OrderByDescending(x => x.VentasDelTipo).Take(5);

            List<DTOEstadisticasTipoComponente> result = await orderedQuery.ToListAsync();
            return result;
        }

        public async Task<List<DTOVentasComponentes>> GetVentasComponentes()
        {
            var ventasComponentes = await(from detalle in _context.DetallesPedidos
                                          join componente in _context.Componentes on detalle.IdComponente equals componente.IdComponente
                                          join marca in _context.Marcas on componente.IdMarca equals marca.IdMarca
                                          group detalle by new { marca.NombreMarca, componente.Nombre } into g
                                          where g.Sum(dp => dp.Cantidad) > 0  // Solo los componentes que se han vendido
                                          select new DTOVentasComponentes
                                          {
                                              MarcaComponente = g.Key.NombreMarca,
                                              NombreComponente = g.Key.Nombre,
                                              TotalUnidadesVendidas = g.Sum(dp => dp.Cantidad),
                                              IngresoTotalGenerado = g.Sum(dp => dp.Cantidad * dp.PrecioUnitario)
                                          })
                                 .OrderByDescending(v => v.IngresoTotalGenerado).Take(5)
                                 .ToListAsync();

            return ventasComponentes;
        }

        public async Task<List<DTOFacturacionPorMes>> GetFacturacionPorMes(int? year = null)
        {
            var facturacionPorMesQuery = from detalle in _context.DetallesPedidos
                                         join pedido in _context.Pedidos on detalle.IdPedido equals pedido.IdPedido
                                         select new { detalle, pedido };

            // Si se pasó un año, filtramos por el año recibido; si no, no filtramos por año
            if (year.HasValue)
            {
                facturacionPorMesQuery = facturacionPorMesQuery.Where(x => x.pedido.FechaPedido.Year == year.Value);
            }

            var facturacionPorMes = await facturacionPorMesQuery
                .GroupBy(x => x.pedido.FechaPedido.Month) // Agrupar por mes
                .Select(g => new DTOFacturacionPorMes
                {
                    Mes = g.Key,
                    Facturacion = g.Sum(dp => dp.detalle.Cantidad * dp.detalle.PrecioUnitario)
                })
                .OrderBy(v => v.Mes) // Ordenar por mes (1 a 12)
                .ToListAsync();

            return facturacionPorMes;
        }

        // TARJETAS
        public async Task<int> GetNumeroVentasMesActual()
        {
            var cantidadPedidos = await _context.Pedidos
                .Where(p => p.FechaPedido.Month == DateTime.Now.Month && p.FechaPedido.Year == DateTime.Now.Year)
                .CountAsync(); 

            return cantidadPedidos;
        }

        public async Task<string> FacturacionDeEsteMes()
        {
            var cultura = CultureInfo.GetCultureInfo("es-AR"); // Cultura para formateo

            decimal totalVentasMesActual = await _context.Pedidos
                .Where(p => p.FechaPedido.Month == DateTime.Now.Month && p.FechaPedido.Year == DateTime.Now.Year)
                .Join(_context.DetallesPedidos, p => p.IdPedido, dp => dp.IdPedido, (p, dp) => new { dp.Cantidad, dp.PrecioUnitario })
                .SumAsync(dp => dp.Cantidad * dp.PrecioUnitario);

            // Formatear como string con separadores
            return string.Format(cultura, "{0:N2}", totalVentasMesActual);
        }


        public Task<int> GetCantidadClientes()
        {
            var clientes = _context.Clientes.Select(c => c.IdCliente).CountAsync();

            return clientes;
        }

        public async Task<List<PedidosMasGrandesDTO>> ObtenerPedidosConMayorImporte()
        {
            var fechaActual = DateTime.Now.Year;
            var cultura = CultureInfo.GetCultureInfo("es-AR");

            var resultado = await (from p in _context.Pedidos
                                   join dp in _context.DetallesPedidos on p.IdPedido equals dp.IdPedido
                                   join c in _context.Clientes on p.IdCliente equals c.IdCliente
                                   where p.FechaPedido.Year == fechaActual
                                   group dp by new { p.IdPedido, c.Apellido, c.Nombre, p.FechaPedido } into g
                                   orderby g.Sum(dp => (dp.Cantidad * dp.PrecioUnitario) - dp.Descuento) descending
                                   select new PedidosMasGrandesDTO
                                   {
                                       IdPedido = g.Key.IdPedido,
                                       Cliente = g.Key.Apellido + " " + g.Key.Nombre,
                                       FechaPedido = g.Key.FechaPedido, // Mantener como DateTime
                                       Importe = string.Format(cultura, "{0:N2}", g.Sum(dp => (dp.Cantidad * dp.PrecioUnitario) - dp.Descuento))
                                   }).Take(5).ToListAsync();

            // Si necesitas formatear la fecha al devolverla, hazlo en la capa de presentación o formatea al mostrar
            return resultado;
        }




        public async Task<List<Ultimos5ClientesDTO>> Ultimos5Clientes()
        {
            var cultura = CultureInfo.GetCultureInfo("es-AR"); // Cultura para formateo

            var resultado = await (from p in _context.Pedidos
                                   join c in _context.Clientes on p.IdCliente equals c.IdCliente
                                   join dp in _context.DetallesPedidos on p.IdPedido equals dp.IdPedido
                                   join fp in _context.FormasPagos on p.IdFormaPago equals fp.IdFormaPago
                                   group dp by new { p.IdPedido, c.Apellido, c.Nombre, p.FechaPedido, fp.NombreFormaPago } into g
                                   orderby g.Key.FechaPedido descending
                                   select new Ultimos5ClientesDTO
                                   {
                                       Cliente = g.Key.Apellido + " " + g.Key.Nombre,
                                       FormaPago = g.Key.NombreFormaPago,
                                       FechaPedido = g.Key.FechaPedido, // Dejar como DateTime
                                       Importe = string.Format(cultura, "{0:N2}", g.Sum(dp => (dp.Cantidad * dp.PrecioUnitario) - (dp.Descuento ?? 0)))
                                   }).Take(5).ToListAsync();

            return resultado;
        }








    }
}
