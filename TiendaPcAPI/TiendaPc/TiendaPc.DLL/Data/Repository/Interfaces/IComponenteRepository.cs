using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaPc.DLL.Models;

namespace TiendaPc.DLL.Data.Repository.Interfaces
{
    public interface IComponenteRepository
    {
        Task<List<Componente>> GetAllAsync();

        Task<List<Componente>> GetAllFiltrosAsync(int? idMarca, int? idTipo, bool? estado);
        
        Task<Componente> GetByIdAsync(int id);
        Task<bool> UpadateAsycn(Componente componente);
        Task<bool> CreateAsycn(Componente componente);
        Task<bool> LowComponenteAsycn(int id);


        // testing no tomar en cuenta
        Task<bool> nuevaEspecificacion(Especificacion especificacion);
        Task<Especificacion> GetEspecificacionById(int id);

        Task<List<TipoComponenteEspecificacion>> GetAllTipoEspecificacion(int idComponente);

        Task<List<EspecificacionComponente>> GetAllEspecificacionesByIdComp(int idComponente);

        Task<bool> nuevaEspecificacionComponente(EspecificacionComponente specComponente);
    }
}
