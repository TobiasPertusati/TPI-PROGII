﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace TiendaPc.DLL.Models;

public partial class TipoComponente
{
    public int IdTipoComponente { get; set; }

    public string Tipo { get; set; }

    public virtual ICollection<Componente> Componentes { get; set; } = new List<Componente>();

    public virtual ICollection<TipoComponenteEspecificacion> TiposComponentesEspecificaciones { get; set; } = new List<TipoComponenteEspecificacion>();
}