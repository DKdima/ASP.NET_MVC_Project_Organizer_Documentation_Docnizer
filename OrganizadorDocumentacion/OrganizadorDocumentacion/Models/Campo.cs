using System;
using System.Collections.Generic;

namespace OrganizadorDocumentacion.Models;
/*
public partial class Campo
{
    public string Nombre { get; set; } = null!;

    public int? CantidadArchivos { get; set; }

    public int CampoId { get; set; }

    public int? IniciativaId { get; set; }

    public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    public virtual Iniciativa? Iniciativa { get; set; }

    public virtual ICollection<Subcampo> Subcampos { get; set; } = new List<Subcampo>();
}*/
public partial class Campo
{
    public int CampoId { get; set; }

    public string Nombre { get; set; } = null!;

    public int? CantidadArchivos { get; set; }

    public int? IniciativaId { get; set; }

    public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    public virtual Iniciativa? Iniciativa { get; set; }

    public virtual ICollection<Subcampo> Subcampos { get; set; } = new List<Subcampo>();
    public string Ref { get; set; }
}
