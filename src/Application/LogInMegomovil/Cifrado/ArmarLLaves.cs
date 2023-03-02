
using System.Xml;
using Domain.Entities;

namespace Application.LogInMegomovil
{
    public class ArmarLLaves
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
    }
}
