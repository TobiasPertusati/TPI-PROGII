﻿using TiendaPc.DLL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TiendaPc.DLL.Services.Interfaces
{
    public interface IEspecificacionService
    {
        Task<List<Especificacion>> GetAllAsync();
    }
}