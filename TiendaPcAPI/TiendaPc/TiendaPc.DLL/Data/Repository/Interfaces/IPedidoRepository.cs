using TiendaPc.DLL.Models;
using TiendaPc.DLL.Models.DTO;

namespace TiendaPc.DLL.Data.Repository.Intefaces
{
    public interface IPedidoRepository
    {
        Task<List<Pedido>> GetAll();
        Task<Pedido> GetById(int id);
        Task<List<DetallePedidoDto>> GetAllDetallesPedido(int idPedido);
        Task<PedidoDto> GetByIdPedidoDto(int id);
        Task<List<PedidoDto>> GetPedidosFiltros(DateTime? fechaDesde, DateTime? fechaHasta, int? idFormaPago,string? estado); 
        Task<List<Pedido>> GetPedidosPCArmadas();
        Task<bool> Save(Pedido pedido); //post pedido
        //Task<bool> Update(int id, Pedido pedido);
        Task<bool> LowOrder(int id, string motivoCancelacion);//baja logica, estableciendo en false el estado del pedido
        Task<bool> ConfrimrOrder(int id);//establecer pedido entregado.
        public Task<bool> AsignarGamerCoins(int idCliente, decimal total, int cantidad);
        public Task<bool> DescontarGamerCoins(int idCliente);

    }
}
