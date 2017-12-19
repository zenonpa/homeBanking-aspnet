using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
    public class enOficina : enBase
    {
        public int IdOficina { set; get; }
        public int IdZona { set; get; }
        public string NombreOficina { set; get; }
        public string NombreZona { set; get; }
        public int CodigoEstado { set; get; }
        public bool Eliminado { set; get; }
        public int iCodigoZona { set; get; }
        public string Zona { set; get; }
        public int iCodigoOficina { set; get; } //CodigoOficina
        public string Oficina { set; get; } //NombreOficina

    }
}
