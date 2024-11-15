using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaPc.DLL.Data.Repository.Interfaces;
using TiendaPc.DLL.Models;
using TiendaPc.DLL.Services.Interfaces;

namespace TiendaPc.DLL.Services
{
    public class ComponenteService : IComponenteService
    {
        private readonly IComponenteRepository _repository;
        public ComponenteService(IComponenteRepository repository)
        {
            _repository = repository;   
        }

        public async Task<bool> CreateAsycn(Componente componente)
        {
           return await _repository.CreateAsycn(componente);
        }

        public async Task<List<Componente>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Componente>> GetAllFiltrosAsync(int? idMarca, int? idTipo, bool? estado)
        {
            return await _repository.GetAllFiltrosAsync(idMarca, idTipo, estado);
        }

        public async Task<Componente> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> LowComponenteAsycn(int id)
        {
            return await _repository.LowComponenteAsycn(id);
        }



        public async Task<bool> UpadateAsycn(Componente componente)
        {
            return await _repository.UpadateAsycn(componente);
        }
        public async Task<Especificacion> GetEspecificacionById(int id)
        {
            return await _repository.GetEspecificacionById(id);
        }
        public async Task<bool> nuevaEspecificacion(Especificacion especificacion)
        {
            return await _repository.nuevaEspecificacion(especificacion);
        }


        public async Task<List<TipoComponenteEspecificacion>> GetAllTipoEspecificacion(int idComponente)
        {
            return await _repository.GetAllTipoEspecificacion(idComponente);
        }

        public async Task<List<EspecificacionComponente>> GetAllEspecificacionesByIdComp(int idComponente)
        {
            return await _repository.GetAllEspecificacionesByIdComp(idComponente);
        }

        public async Task<bool> nuevaEspecificacionComponente(EspecificacionComponente specComponente)
        {
            return await _repository.nuevaEspecificacionComponente(specComponente);
        }
    }
}
