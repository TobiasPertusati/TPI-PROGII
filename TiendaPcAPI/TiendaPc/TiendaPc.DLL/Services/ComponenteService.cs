using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaPc.DLL.Data.Repository.Interfaces;
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


    }
}
