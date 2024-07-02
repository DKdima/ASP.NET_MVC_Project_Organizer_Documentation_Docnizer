namespace OrganizadorDocumentacion.DTOs
{
    public class CampoDTO
    {
        public string NombreCampo { get; set; }
        public int? CantidadArchivos { get; set; }
        public List<SubcampoDTO> Subcampos { get; set; }
    }
}
