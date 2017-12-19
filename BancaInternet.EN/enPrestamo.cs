using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
    public class enPrestamo
    {
        public string sCodigoSiscredinka {get;set;}
		public decimal nTipoCodigoProducto { get; set; }
        public string sNombreProducto {get;set;}
        public string sNumeroCredito {get;set;}
        public string sSaldoCapital {get;set;}
		public string sEstadoCuenta { get; set; }
        public string sCodigoEstado { get; set; }
		public string sFechaVencimiento { get; set; }
		public Int16 nTotalCuotas { get; set; }
		public Int16 nTotalCuotasPendientes { get; set; }
		public string sTipoCredito { get; set; }
		public string sModalidadCredito { get; set; }
		public string sTEA { get; set; }
		public int iDiasAtraso { get; set; }
    }
}
