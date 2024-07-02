using System;
using System.Collections.Generic;

namespace OrganizadorDocumentacion.Models;

public partial class Usuario
{
    public string Nombre { get; set; } = null!;

    public string? Clave { get; set; }

    public string? TipoUsuario { get; set; }

    public int UsuarioId { get; set; }

    public virtual ICollection<UsuarioIniciativa> UsuarioIniciativas { get; set; } = new List<UsuarioIniciativa>();
}
