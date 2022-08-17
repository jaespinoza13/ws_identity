using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Application.ParametrosSeguridad;

public class GenRsaPublicaPrivada
{
    RSACryptoServiceProvider rsa = null;
    public bool GenerateKeys(
       out string privateKey,
       out string publicKey,
       int keySize = 2048)
    {
        RSA rsa = RSA.Create(keySize);

        // Public key.
        var pubKey = rsa.ExportParameters( false );

        var stringWriter = new StringWriter();
        var serializer = new XmlSerializer( typeof( RSAParameters ) );

        serializer.Serialize( stringWriter, pubKey );
        publicKey = stringWriter.ToString();

        // Private key.
        var privKey = rsa.ExportParameters( true );

        stringWriter = new StringWriter();
        serializer = new XmlSerializer( typeof( RSAParameters ) );
        serializer.Serialize( stringWriter, privKey );
        privateKey = stringWriter.ToString();
        return true;
    }
  
   
}
