using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
    public class enCuenta : enBase
    {
        public string sCodigoSiscredinka {get;set;}
		public decimal nTipoCodigoProducto { get; set; }
        public string sNombreProducto {get;set;}
        public string sNumeroCuenta {get;set;}
		public string sSaldo { get; set; }
        public string sEstadoCuenta {get;set;}
        public string sNombreCompletoCliente { get; set; }
        public string sCodigoEstado { get; set; }
		public string sProducto { get; set; }
		public string sIdentificador { get; set; }
		public string sMontoMovimiento { get; set; }
		public string sSaldoDisponible { get; set; }
		public string sSaldoContable { get; set; }
		public decimal nNumeroTransaccion { get; set; }
		public string sFechaMovimiento { get; set; }
		public string sSigno { get; set; }
		public string sIndicadorMovimiento { get; set; }
		public string sDescripcionMovimiento { get; set; }
		public string sFechaVencimiento { get; set; }
		public string sTipoMoneda { get; set; }
		public string sCanal { get; set; }
    }
}
