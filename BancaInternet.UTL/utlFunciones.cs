using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Security.Cryptography;

namespace BancaInternet.UTL
{
    public class utlFunciones
    {

        private static string KeyMaster = ConfigurationManager.AppSettings["KeyMaestra"];

        public static string P_RJD_Encriptar(string strEncriptar, string sKEY)
        {
            Rijndael oRijndael = Rijndael.Create();
            string returnValue = "";
            int keySize = 32;
            int ivSize = 16;

            try
            {
                if (!strEncriptar.Equals(""))
                {
                    byte[] key = UTF8Encoding.UTF8.GetBytes(sKEY);

                    byte[] iv = UTF8Encoding.UTF8.GetBytes("Cr3d1Nk@");

                    Array.Resize<byte>(ref key, keySize);
                    Array.Resize<byte>(ref iv, ivSize);

                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    oRijndael.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);

                    byte[] plainMessageBytes = UTF8Encoding.UTF8.GetBytes(strEncriptar);
                    cryptoStream.Write(plainMessageBytes, 0, plainMessageBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    returnValue = Convert.ToBase64String(memoryStream.ToArray());
                    memoryStream.Close();
                    cryptoStream.Close();
                }
            }
            catch (Exception ex)
            {
                returnValue = "";
            }
            finally
            {
                oRijndael.Clear();
            }
            return returnValue;
        }

        public static string P_RJD_Desencriptar(string strDesEncriptar, string sKEY)
        {
            Rijndael miRijndael = Rijndael.Create();
            string returnValue = "";
            int keySize = 32;
            int ivSize = 16;

            try
            {
                if (!strDesEncriptar.Equals(""))
                {
                    byte[] key = UTF8Encoding.UTF8.GetBytes(sKEY);
                    byte[] iv = UTF8Encoding.UTF8.GetBytes("Cr3d1Nk@");

                    Array.Resize<byte>(ref key, keySize);
                    Array.Resize<byte>(ref iv, ivSize);

                    byte[] cipherTextBytes = Convert.FromBase64String(strDesEncriptar);
                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                    MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                    CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                                 miRijndael.CreateDecryptor(key, iv),
                                                                 CryptoStreamMode.Read);

                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                    memoryStream.Close();
                    cryptoStream.Close();

                    returnValue = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                }


            }
            catch
            {
                returnValue = "";
            }
            finally
            {
                miRijndael.Clear();
            }
            return returnValue;
        }

        public static string Encriptar_PWD(string strTexto)
        {
            string sResult = "";
            try 
            {
                byte[] bKey = Encoding.UTF8.GetBytes(KeyMaster);
                HMACSHA1 _objHMACSHA = new HMACSHA1(bKey);
                byte[] bResult = _objHMACSHA.ComputeHash(Encoding.UTF8.GetBytes(strTexto));
                sResult = Convert.ToBase64String(bResult);
                _objHMACSHA.Clear();
            }

            catch (Exception ex)
            {
                sResult = "";
            }
            return sResult;
        }

        public static bool CompararPWD(string pwd1, string pwd2)
        {
            bool bResult = true;
            try
            {
                byte[] sContra1 = Convert.FromBase64String(pwd1);
                byte[] sContra2 = Convert.FromBase64String(pwd2);

                if (sContra1.Length != sContra2.Length)
                {
                    bResult = false;
                }

                for (int i = 0; i <= sContra1.Length - 1; i++)
                {
                    if (!sContra1[i].Equals(sContra2[i]))
                    {
                        bResult = false;
                    }

                }
            }
            catch (Exception ex)
            {
                bResult = false;
            }
            return bResult;
        }

        public static string RJD_Encriptar(string strEncriptar, string strPK)
        {
            return P_RJD_Encriptar(strEncriptar, strPK);
        }

        public static string RJD_Desencriptar(string strDesEncriptar, string strPK)
        {
            return P_RJD_Desencriptar(strDesEncriptar, strPK);
        }
    }
}
