using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BancaInternet.UTL;

namespace BancaInternet.Funciones
{
    public class rnSegRSA
    {
        private static string LlavePublica = "<RSAKeyValue><Modulus>7eqDlsm/8e5804CpYs03A4D9Krw14YiHLqhLxEpd32ju677ANWZKYG8OCYedcl4ueBTElCVwqo3oqJ5xLPT7C3E+MaUf"+
                                             "p/31rzoFgOaxBnKjhXjxmP+wKOZjV3AoSczssurxFyXWFbUTyqaSey4yCEJI3bZoWLcAas69IPjVMhk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

       public string RSA_Encriptar(string sTexto)
       {
           string sResultado = "";

           try 
           {
               byte[] _bytEncriptado = null;

               //Creamos una instancia del encritador publico 
               RSACryptoServiceProvider _objEncriptadorPublico = new RSACryptoServiceProvider(1024);
               //Le asignamos la llave genarada 
               _objEncriptadorPublico.FromXmlString(LlavePublica);

               //Se declara la memoria para almacenar la llave utilizada por nuestro Rijndael personalizado 
               byte[] bKEY = (Rijndael.Create()).Key;
               string sKEY = Convert.ToBase64String(bKEY);

               //Se encripta el texto y se obtiene la llave que se utilizó para la encriptación 
               string sTextEncriptado_RJ = utlFunciones.RJD_Encriptar(sTexto, sKEY);
               byte[] _bytEncriptadoSimetrico = Convert.FromBase64String(sTextEncriptado_RJ);

               //Se encripta la llave con el algoritmo RSA 
               byte[] _bytEncriptadoLlave = _objEncriptadorPublico.Encrypt(bKEY, false);

               //Se copia en un arreglo la llave encriptada y el encriptado de Rijndael 
               _bytEncriptado = new byte[_bytEncriptadoLlave.Length + _bytEncriptadoSimetrico.Length];
               _bytEncriptadoLlave.CopyTo(_bytEncriptado, 0);
               _bytEncriptadoSimetrico.CopyTo(_bytEncriptado, _bytEncriptadoLlave.Length);

               sResultado = Convert.ToBase64String(_bytEncriptado);
           }
           catch (Exception ex) 
           {
               sResultado = "";
           }
           return sResultado;
       }

    }
}
