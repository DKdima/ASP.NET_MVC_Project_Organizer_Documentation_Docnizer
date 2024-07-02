namespace OrganizadorDocumentacion.DTOs
{
    public class SubcampoDTO
    {
        public string NombreSubcampo { get; set; }
        public int? CantidadArchivos { get; set; }
        public int? ParentSubcampoId { get; set; }
        public List<SubcampoDTO> Subsubcampos { get; set; }
    }
}
