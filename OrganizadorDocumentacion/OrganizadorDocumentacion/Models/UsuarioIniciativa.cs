using System;
using System.Collections.Generic;

namespace OrganizadorDocumentacion.Models;

public partial class UsuarioIniciativa
{
    public int UsuarioIniciativaId { get; set; }

    public int? UsuarioId { get; set; }

    public int? IniciativaId { get; set; }

    public virtual Iniciativa? Iniciativa { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
