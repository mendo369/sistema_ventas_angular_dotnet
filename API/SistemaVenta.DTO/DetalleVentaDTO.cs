using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DTO
{
    public class DetalleVentaDTO
    {
        public int? IdProducto { get; set; }

        public string? DescripcionProducto { get; set; }

        public int? Cantidad { get; set; }

        //recibimos en string y luego pasamos a decimal
        public string? PrecioTexto { get; set; }

        public string? TotalTexto { get; set; }
    }
}
