using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BancaInternet.EN
{
	public class enUsuario : enBase
	{

		public string sCodigoSisCredinkaCliente { get; set; }
		public int iBloqueo { get; set; }
		public int iSello { get; set; }
		public int iCorrespondencia { get; set; }
		public string sNumeroTarjeta { get; set; }
		public string sPing { get; set; }
		public string sTipoCambioCompra { get; set; }
		public string sTipoCambioVenta { get; set; }
		public string sFechaCambio { get; set; }
		public string sUltimoAcceso { get; set; }
		public int iTipoResultado { get; set; }
		public int iTipoDeAcceso { get; set; }
		public int iTipoTarjeta { get; set; }
		public int iCondicionTarjeta { get; set; }
		public int iCondicionCliente { get; set; }
		public int iTipoDocumento { get; set; }
		public bool bResultado { get; set; }
		public bool bCambioClave { get; set; }

		#region Datos Clientes

		public string sPrimerNombre { get; set; }
		public string sSegundoNombre { get; set; }
		public string sApellidoMaterno { get; set; }
		public string sApellidoPaterno { get; set; }
		public string sEmail { get; set; }
		public string sTipoDocumento { get; set; }
		public string sNumeroDocumento { get; set; }
		public string sFechaNacimiento { get; set; }

		public string sUbigeoNacimiento_Dep { get; set; }
		public string sUbigeoNacimiento_Prov { get; set; }
		public string sUbigeoNacimiento_Dist { get; set; }
		public string sNombreUbigeoNacimiento { get; set; }

		public string sDireccion { get; set; }
		public string sReferenciaUbicacion { get; set; }


		public string sUbigeoCasa_Dep { get; set; }
		public string sUbigeoCasa_Prov { get; set; }
		public string sUbigeoCasa_Dist { get; set; }
		public string sNombreUbigeoCasa{ get; set; }

		public decimal nNumeroCasa { get; set; }
		public string sCodigoCIIU { get; set; }
		public string sNombreActividadCIIU { get; set; }
		public string sRazonSocial { get; set; }
		public string sRuc { get; set; }
		public string sCodigoCargo { get; set; }
		public string sNombreCargo { get; set; }
		public DateTime dtFechaNacimiento { get; set; }
		public DateTime dtFechaIngreso { get; set; }
		public string sFechaIngreso { get; set; }

		public string sUbigeoLaboral_Dep { get; set; }
		public string sUbigeoLaboral_Prov { get; set; }
		public string sUbigeoLaboral_Dist { get; set; }
		public string sNombreUbigeoLaboral { get; set; }

		public string sDireccionLaboral { get; set; }
		public string sUrbanizacionLaboral { get; set; }
		public string sCodigoUbigeoLaboral { get; set; }
		public decimal nTelefonoLaboral1 { get; set; }
		public decimal nTelefonoLaboral2 { get; set; }
		public string sCodigoRelacionLaboral { get; set; }
		public int iIdentificarAut { get; set; }
		public int iIdentificarPep { get; set; }
		public int iEnvioCuenta { get; set; }
		public int iEnvioReciboEECC { get; set; }
		public string sPepOcupacion { get; set; }
		public string sPepOrganismo { get; set; }
		public string sCodigoTipoVia { get; set; }
		public string sCodigoTipoZona { get; set; }
		public string sNombreZona { get; set; }
		public string sNroVia { get; set; }
		public int iIndicadorPep { get; set; }
		public string sAudOficina { get; set; }
        public int iIdOficina { get; set; }


		public enUsuario()
		{

			sPrimerNombre = string.Empty;
			sSegundoNombre = string.Empty;
			sApellidoMaterno = string.Empty;
			sApellidoPaterno = string.Empty;
			sEmail = string.Empty;
			sTipoDocumento = string.Empty;
			sNumeroDocumento = string.Empty;
			sFechaNacimiento = string.Empty;
			sDireccion = string.Empty;
			sReferenciaUbicacion = string.Empty;
			nNumeroCasa = 0;
			sCodigoCIIU = string.Empty;
			sNombreActividadCIIU = string.Empty;
			sRazonSocial = string.Empty;
			sRuc = string.Empty;
			sCodigoCargo = string.Empty;
			sNombreCargo = string.Empty;
			dtFechaNacimiento = Convert.ToDateTime("01/01/1900");
			dtFechaIngreso = Convert.ToDateTime("01/01/1900");
			sFechaIngreso = string.Empty;
			sDireccionLaboral = string.Empty;
			sUrbanizacionLaboral = string.Empty;
			sCodigoUbigeoLaboral = string.Empty;
			nTelefonoLaboral1 = 0;
			nTelefonoLaboral2 = 0;
			sCodigoRelacionLaboral = string.Empty;
			iIdentificarAut = -1;
			iIdentificarPep = -1;
			iEnvioCuenta = -1;
			iEnvioReciboEECC = -1;
            iIdOficina = 0;
		}
		#endregion
	}
}
