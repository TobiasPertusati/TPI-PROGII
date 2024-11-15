using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaPc.API.DTO;
using TiendaPc.DLL.DTO;
using TiendaPc.DLL.Models.DTO;

namespace TiendaPc.DLL.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<List<DTOVentasMarcas>> GetEstadisticasMarcas();
        Task<List<DTOEstadisticasTipoComponente>> GetDTOEstadisticasTipoComponentes();
        Task<List<DTOVentasComponentes>> GetVentasComponentes();
        Task<List<DTOFacturacionPorMes>> GetFacturacionPorMes(int? year = null);
        Task<int> GetNumeroVentasMesActual();
        Task<string> FacturacionDeEsteMes();
        Task<int> GetCantidadClientes();
        Task<List<PedidosMasGrandesDTO>> ObtenerPedidosConMayorImporte();
        Task<List<Ultimos5ClientesDTO>> Ultimos5Clientes();
    }
}
