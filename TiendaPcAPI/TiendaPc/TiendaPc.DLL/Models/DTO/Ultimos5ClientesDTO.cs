using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaPc.DLL.Models.DTO
{
    public class Ultimos5ClientesDTO
    {
        public string? Cliente { get; set; }
        public string? FormaPago { get; set; }
        public DateTime? FechaPedido { get; set; }
        public string? Importe { get; set; }
    }

}
