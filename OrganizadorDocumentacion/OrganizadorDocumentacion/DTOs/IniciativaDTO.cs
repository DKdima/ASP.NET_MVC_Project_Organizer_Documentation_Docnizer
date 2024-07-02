using static OrganizadorDocumentacion.Controllers.HomeController;

namespace OrganizadorDocumentacion.DTOs
{
    public class IniciativaDTO
    {
        public string NombreIniciativa { get; set; }
        public List<CampoDTO> Campos { get; set; }
    }
}
