using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
	public class enUsuarioValida
	{
        public enUsuarioValida() {
            bResultado = false;
            sCodigoCliente = "";
        }

		public bool bResultado { get; set; }
		public string sCodigoCliente { get; set; }
		public int iTipoTarjeta { get; set; }
		public string sNumeroTarjeta { get; set; }
		public string sPing { get; set; }
		public string sPingNuevo { get; set; }
		public int iBloqueo { get; set; }
		public string vAudIPCreacion { get; set; }
		public string vAudMACCreacion { get; set; }
		public int iTipoResultado { get; set; }
		public int iCondicionTarjeta { get; set; }
		public int iCondicionCliente { get; set; }
		public string sEmail { get; set; }
        public int iIdOficina { get; set; }
	}
}
