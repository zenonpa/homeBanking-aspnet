using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancaInternet.EN
{
    public class enBase
    {
		public int RowNumber { set; get; }
		public int NumeroPagina { set; get; }
		public int NumeroRegistros { set; get; }
		public int TotalRegistros { set; get; }
		public int AjaxResultado { set; get; }
		public string AjaxError { set; get; }
		public string vAudNombreUsuarioCreacion { set; get; }
		public DateTime dtAudFechaCreacion { set; get; }
		public string vAudIPCreacion { set; get; }
		public string vAudMACCreacion { set; get; }
		public string vAudNombreUsuarioModificacion { set; get; }
		public DateTime dtAudFechaModificacion { set; get; }
		public string vAudIPModificacion { set; get; }
		public string vAudMACModificacion { set; get; }
		public string vAudFechaCreacion { set; get; }
		public string vAudFechaModificacion { set; get; }
		public bool bEliminado { get; set; }
    }
}
