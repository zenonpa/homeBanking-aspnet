using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.UTL
{
    public class utlConstante
    {
        //Tipo LOG
        public const string LogTipoDebug = "DEBUG";
        public const string LogTipoInfo = "INFO";
        public const string LogTipoWarning = "WARN";
        public const string LogTipoError = "ERROR";
        public const string LogTipoFatal = "FATAL";
		public const string ResultadoOK = "OK";
        //Bibliteca.
        public const string ModuloBancaWSE = "BancaWSE";
		public const string ModuloBancaAD = "BancaAD";
        public const string ModuloBancaUTL = "BancaUTL";
        public const string ModuloBancaWEB = "BancaWEB";

        
        //LOG SPACE
        public const string LogNamespace_WSE = "BancaInternet_WSE";
		public const string LogNamespace_AD = "BancaInternet_AD";
        public const string LogNamespace_WEB = "BancaInternet_WEB";
        public const string LogNamespace_FUN = "BancaInternet_UTL";
		public const string LogGeneracionPing = "GeneracionPING";

		//Tarjetas
		public const int iTipoTarjeta = 1;  //Tarjeta de Debito 
		public enum EstadoUsuario { NoBloqueado = 0, Bloqueado = 1 };
		public enum EstadoTarjeta { Activo = 1, Perdida = 2, Bloqueada = 3, Robada = 10, Cancelada = 50, SinCambioClave = 99, NoExiste = 999 };
		public enum EstadoAfiliacionP1
		{
			ExitoOperacionP1 = 1,
			Error_Tarjeta_Perdida = 20,
			Error_Tarjeta_Bloqueda = 30,
			Error_Tarjeta_Robada = 10,
			Error_Tarjeta_Cancelada = 50,
			Error_Tarjeta_SinCambioClave = 9,
			Error_Tarjeta_NoExiste = 99,
			Error_Cliente_Eliminado = 2,
			Error_Cliente_Bloqueado = 3,
			Error_Cliente_Asociado = 4,
			Error_Cliente_NoExiste = 999
		}

		public enum EstadoActualizacionAP2 { ActualizacionExistosa = 1, NoExisteCliente = 2, ErrorActualiza = 3 };
		public enum EstadoAfiliacionP2 { AfiliacionExito = 1, AfiliacionExiste = 2, Error = 3 };
		//Clientes
		public enum EstadoCliente { Activo = 1, EliminadoLogico = 2, Bloqueado = 3, AsociadoHomeB = 4 }
		public enum EstadoLogeo { Exitoso = 1, Erroneo = 2, ClienteSinAsociar = 3 };
		public enum EstadoGeneracion { Exitoso = 1, ErrorValidacion = 2, ClienteAsociado = 3,ClienteSinAsociar= 4 };
		public enum EstadoCambioClave { CoincideExito = 1, NoCoincide = 0 };
		public const int iTipoDeAccesoWeb = 1;
		public const int Timeout = 60;

        //Tipo de Archivo
        public const int TipoArchivo_Carrusel = 1;
        public const int TipoArchivo_Faq = 5;

    }
}
