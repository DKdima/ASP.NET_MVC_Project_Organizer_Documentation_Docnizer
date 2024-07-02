using System;
using System.Collections.Generic;

namespace OrganizadorDocumentacion.Models;

public partial class Archivo
{

    public int ArchivoId { get; set; }

    public int? CampoId { get; set; }

    public int? SubcampoId { get; set; }

    public virtual Campo? Campo { get; set; }

    public virtual Subcampo? Subcampo { get; set; }
    // Propiedades adicionales para la visualización en la tabla
    public string Nombre { get; set; }
    public string FechaModificacion { get; set; }

    public string Autor { get; set; }
    public long? Tamano { get; set; }
}
