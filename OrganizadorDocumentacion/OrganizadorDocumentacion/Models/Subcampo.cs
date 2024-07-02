using System;
using System.Collections.Generic;

namespace OrganizadorDocumentacion.Models;
/*
public partial class Subcampo
{
    public string Nombre { get; set; } = null!;

    public int? CantidadArchivos { get; set; }

    public int SubcampoId { get; set; }

    public int? ParentCampoId { get; set; }

    public int? ParentSubcampoId { get; set; }

    public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    public virtual ICollection<Subcampo> InverseParentSubcampo { get; set; } = new List<Subcampo>();
    public List<Subcampo> Subcampos { get; set; } // Recursivo para subcampos de subcampos
    public virtual Campo? ParentCampo { get; set; }

    public virtual Subcampo? ParentSubcampo { get; set; }
}*/
public partial class Subcampo
{
    public int SubcampoId { get; set; }

    public string Nombre { get; set; } = null!;

    public int? CantidadArchivos { get; set; }

    public int? ParentCampoId { get; set; }

    public int? ParentSubcampoId { get; set; }

    public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    public virtual ICollection<Subcampo> InverseParentSubcampo { get; set; } = new List<Subcampo>();

    public virtual Campo? ParentCampo { get; set; }

    public virtual Subcampo? ParentSubcampo { get; set; }
    public string Ref { get; set; }
}
