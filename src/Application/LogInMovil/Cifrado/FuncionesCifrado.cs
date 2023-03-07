
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Domain.Entities;

namespace Application.LogInMegomovil
{
    public class FuncionesCifrado
    {
        public static void armarLlaves ( LlavesCifradoMegomovil dt, ref string srt_llave_pub_pri_xml, ref string str_iv )
        {
            XmlDocument doc = new XmlDocument( );
            XmlElement root = doc.DocumentElement!;
            XmlElement RSAKeyValue = doc.CreateElement(string.Empty, "RSAKeyValue", string.Empty);
            doc.AppendChild(RSAKeyValue);

            XmlElement Modulus = doc.CreateElement(string.Empty, "Modulus", string.Empty);
            XmlText str_modulo = doc.CreateTextNode(dt.lci_modulo);
            Modulus.AppendChild(str_modulo);
            RSAKeyValue.AppendChild(Modulus);

            XmlElement Exponent = doc.CreateElement(string.Empty, "Exponent", string.Empty);
            XmlText str_exponente = doc.CreateTextNode(dt.lci_exponente);
            Exponent.AppendChild(str_exponente);
            RSAKeyValue.AppendChild(Exponent);

            XmlElement P = doc.CreateElement(string.Empty, "P", string.Empty);
            XmlText str_p = doc.CreateTextNode(dt.lci_p);
            P.AppendChild(str_p);
            RSAKeyValue.AppendChild(P);

            XmlElement Q = doc.CreateElement(string.Empty, "Q", string.Empty);
            XmlText str_q = doc.CreateTextNode(dt.lci_q);
            Q.AppendChild(str_q);
            RSAKeyValue.AppendChild(Q);

            XmlElement DP = doc.CreateElement(string.Empty, "DP", string.Empty);
            XmlText str_dp = doc.CreateTextNode(dt.lci_dp);
            DP.AppendChild(str_dp);
            RSAKeyValue.AppendChild(DP);

            XmlElement DQ = doc.CreateElement(string.Empty, "DQ", string.Empty);
            XmlText str_dq = doc.CreateTextNode(dt.lci_dq);
            DQ.AppendChild(str_dq);
            RSAKeyValue.AppendChild(DQ);

            XmlElement InverseQ = doc.CreateElement(string.Empty, "InverseQ", string.Empty);
            XmlText str_inversaq = doc.CreateTextNode(dt.lci_inversaq);
            InverseQ.AppendChild(str_inversaq);
            RSAKeyValue.AppendChild(InverseQ);

            XmlElement D = doc.CreateElement(string.Empty, "D", string.Empty);
            XmlText str_d = doc.CreateTextNode(dt.lci_d);
            D.AppendChild(str_d);
            RSAKeyValue.AppendChild(D);

            srt_llave_pub_pri_xml = doc.OuterXml;
            str_iv = dt.lci_iv!;
        }

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
