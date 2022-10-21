using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Application.Common.Cryptography;

public class GenRsa
{
    public static bool GenerateKeys(
       out string privateKey,
       out string publicKey,
       int keySize = 1024)
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
