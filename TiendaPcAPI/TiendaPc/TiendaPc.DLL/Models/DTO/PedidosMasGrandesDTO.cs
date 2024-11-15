using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaPc.DLL.Models.DTO
{
    public class PedidosMasGrandesDTO
    {
        public int? IdPedido { get; set; }
        public string? Cliente { get; set; }
        public DateTime? FechaPedido { get; set; }
        public string? Importe { get; set; }
    }

}
