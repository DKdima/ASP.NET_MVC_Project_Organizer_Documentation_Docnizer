namespace OrganizadorDocumentacion.Models
{
    public class IniciativaViewModel
    {
        public int IniciativaId { get; set; }
        public string Nombre { get; set; }
        public int CantidadMaximaArchivos { get; set; }
        public int CantidadActualArchivos { get; set; }

        public double PorcentajeArchivos
        {
            get
            {
                if (CantidadMaximaArchivos == 0) return 0;
                return (double)CantidadActualArchivos / CantidadMaximaArchivos * 100;
            }
        }
    }

}
