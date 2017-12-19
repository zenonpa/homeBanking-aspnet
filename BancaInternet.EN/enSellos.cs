using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
    public class enSellos : enBase
    {
        public int iIdSello { get; set; }
        public string vNombreImagen { get; set; }
        public string vDescripcionImagen { get; set; }
        public string vPesoImagen { get; set; }
        public string vRuta { get; set; }
        public string vAncho { get; set; }
        public string vAlto { get; set; }
        public string vFileServer { get; set; }
        public int iEstado { get; set; }
        public string vFecha { get; set; }
        public string vRutaCompleta { get { return string.Format("{0}{1}", vFileServer, vRuta); } }
        public bool bSeleccion { get; set; }
        public string vNombreArchivo { get; set; }

        public enSellos()
        {
            this.iIdSello = 0;
            this.vNombreImagen = string.Empty;
            this.vDescripcionImagen = string.Empty;
            this.vPesoImagen = string.Empty;
            this.vRuta = string.Empty;
            this.vAncho = string.Empty;
            this.vAlto = string.Empty;
            this.vFileServer = string.Empty;
            this.iEstado = 0;
            this.vFecha = string.Empty;
            this.vNombreArchivo = string.Empty;
            this.bSeleccion = false;
        }
    }

}
