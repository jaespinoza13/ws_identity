using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Application.ParametrosSeguridad
{
    public static class CifradoRSA
    {
        public static DatosLlaveRsa GenerarLlavePublicaPrivada(string NemonicoCanal)
        {
            try
            {
                string srt_llave_priv_xml = string.Empty, srt_llave_pub_xml = string.Empty;
                var obj_grpp = new GenRsaPublicaPrivada();
                var a = obj_grpp.GenerateKeys( out srt_llave_priv_xml , out srt_llave_pub_xml);
                XmlDocument doc = new();
                doc.LoadXml(srt_llave_priv_xml);

                var datosLlave = new DatosLlaveRsa
                {
                    str_modulo = doc.DocumentElement!.SelectSingleNode( "Modulus" )!.InnerText,
                    str_exponente = doc.DocumentElement!.SelectSingleNode( "Exponent" )!.InnerText,
                    str_xml_pub = srt_llave_pub_xml,
                    str_xml_priv = srt_llave_priv_xml

                };
                return datosLlave;
            }
            catch (Exception ex)
            {
                throw new Exception( "Error GenerarLlavePublicaPrivada " + ex );
            }
        }
        public static string Decrypt ( string input, string key )
        {
            var stringReader = new StringReader(key);
            var serializer = new XmlSerializer(typeof(RSAParameters));
            var deskey = (RSAParameters)serializer.Deserialize(stringReader)!;

            var bytes = Decrypt(
                Convert.FromBase64String(input),
                deskey);

            return Encoding.UTF8.GetString(bytes);
        }
        public static byte[] Decrypt ( byte[] input, RSAParameters key )
        {
            using var rsa = RSA.Create(key);

            var bytes = rsa.Decrypt(
                input,
                RSAEncryptionPadding.Pkcs1);

            return bytes;
        }
    }
}
