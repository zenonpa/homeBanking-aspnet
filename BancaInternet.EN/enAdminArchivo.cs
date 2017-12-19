using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
    public class enAdminArchivo: enBase
    {
        public int iIdArchivo { get; set; }
        public int iTipoArchivo { get; set; }
        public string vNombreArchivo { get; set; }
        public string vPesoArchivo { get; set; }
        public string vFileServer { get; set; }
        public string vRutaArchivo { get; set; }
        public int iEstado { get; set; }
        public int iOrden { get; set; }
        public DateTime dFecha { get; set; }
        public string vFecha { get; set; }
        public string vTipo { get; set; }
        public string vTamanio { get; set; }
        public string vRutaCompleta { get { return string.Format("{0}{1}", this.vFileServer, this.vRutaArchivo); } }
        public enAdminArchivo()
        {
            this.iIdArchivo = 0;
            this.iTipoArchivo = 0;
            this.vNombreArchivo = string.Empty;
            this.vPesoArchivo = string.Empty;
            this.vFileServer = string.Empty;
            this.vRutaArchivo = string.Empty;
            this.iEstado = 0;
            this.iOrden = 0;
            this.vTamanio = string.Empty;
            this.vTipo = string.Empty;
            this.vFecha = string.Empty;
        }
    }

}
