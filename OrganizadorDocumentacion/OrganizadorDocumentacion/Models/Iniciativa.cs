using System;
using System.Collections.Generic;

namespace OrganizadorDocumentacion.Models;

public partial class Iniciativa
{
    public int IniciativaId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Campo> Campos { get; set; } = new List<Campo>();

    public virtual ICollection<UsuarioIniciativa> UsuarioIniciativas { get; set; } = new List<UsuarioIniciativa>();
}
