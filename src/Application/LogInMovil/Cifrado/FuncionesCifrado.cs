
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Domain.Entities;

namespace Application.LogInMegomovil
{
    public class FuncionesCifrado
    {
        /// <summary>
        /// Pasa un string hexadecimal a un arreglo de bytes
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray ( string s )
        {
            int i = 0;
            int num = 0;
            checked
            {
                byte[] array = new byte[s.Length / 2 - 1 + 1];
                while (i <= s.Length - 1)
                {
                    array[num] = Convert.ToByte(s.Substring(i, 2), 16);
                    i += 2;
                    num++;
                }
                return array;
            }
        }


        public static string Decrypt ( string llavePubPriXml, byte[] datosEncriptados )
        {
            try
            {
                var rsa = RSA.Create( );
                rsa.KeySize = 1024;
                rsa.FromXmlString(llavePubPriXml);

                var bytes = rsa.Decrypt(datosEncriptados, RSAEncryptionPadding.OaepSHA1);

                return ASCIIEncoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("GenRsaPublicaPrivada.Decrypt: " + ex);
            }
        }
    }
}
