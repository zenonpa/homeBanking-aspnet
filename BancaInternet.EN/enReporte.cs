using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
    public class enReporte: enBase
    {
        public int iCodigoUsuario { get; set; }


        public String sCodigoSolicitud { get; set; }
        public String sCodigoSolicitudAyni { get; set; }
        public decimal dMonto { get; set; }
        public DateTime dFechaDesembolso { get; set; }
        public int iCodigoProducto { get; set; }
        public String sNombreProducto { get; set; }
        public int iCodigoAgencia { get; set; }
        public String sNombreAgencia { get; set; }
        public int iCodigoZona { get; set; }
        public String sNombreZona { get; set; }


        //Filtros
        public string sFechaIni { get; set; }
        public string sFechaFin { get; set; }
        public int iTipoFiltro { get; set; }
        public int iSolicitudAyni { get; set; }
        public int iTipoFinanciamiento { get; set; }
        public int iNumCantidad { get; set; }

        public enReporte()
        {
            this.iCodigoUsuario = 0;
            this.sCodigoSolicitud = string.Empty;
            this.sCodigoSolicitudAyni = string.Empty;
            this.dMonto = 0;
            //DateTime dFechaDesembolso
            this.iCodigoProducto = 0; 
            this.sNombreProducto  = string.Empty;
            this.iCodigoAgencia = 0;
            this.sNombreAgencia  = string.Empty;
            this.iCodigoZona = 0;
            this.sNombreZona = string.Empty;

            //Filtros
            this.sFechaIni = string.Empty;
            this.sFechaFin = string.Empty;
            this.iTipoFiltro = 0;
            this.iSolicitudAyni = 0;
            this.iTipoFinanciamiento = 0; 


        }
    }
}
