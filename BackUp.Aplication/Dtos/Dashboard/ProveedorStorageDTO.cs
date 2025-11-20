using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUp.Application.Dtos.Dashboard
{
    public class ProveedorStorageDTO
    {
        public string Proveedor { get; set; }
        public double UsadoTB { get; set; }
        public double TotalTB { get; set; }
        public string Estado { get; set; }
    }
}
