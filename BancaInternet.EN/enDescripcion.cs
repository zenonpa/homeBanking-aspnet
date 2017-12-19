using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
	public class enDescripcion : enBase
	{
		public int iIdParametro { get; set; }
		public int iIdDominio { get; set; }
		public string sNombre { get; set; }
		public string sDescripcion { get; set; }
		public bool bActivo { get; set; }
		public int iValor1 { get; set; }
		public string sValor1 { get; set; }
		public string sValor2 { get; set; }
		public string sValor3 { get; set; }
		public string sValor4 { get; set; }
		public string sValor5 { get; set; }
		public int iCodigoDominioHijo { get; set; }
		public string sRutaCompleta { get; set; }


	}
}
