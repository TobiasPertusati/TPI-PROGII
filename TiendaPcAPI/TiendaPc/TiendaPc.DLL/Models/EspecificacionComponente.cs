﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace TiendaPc.DLL.Models;

public partial class EspecificacionComponente
{
    public int IdEspecComp { get; set; }

    public int IdComponente { get; set; }

    public int IdEspec { get; set; }

    public string Valor { get; set; }

    public virtual Componente IdComponenteNavigation { get; set; }

    public virtual Especificacion IdEspecNavigation { get; set; }
}