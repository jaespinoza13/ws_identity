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
        RSA csp = RSA.Create();

        // Public key.
        var pubKey = csp.ExportParameters( false );

        var stringWriter = new StringWriter();
        var serializer = new XmlSerializer( typeof( RSAParameters ) );

        serializer.Serialize( stringWriter, pubKey );
        publicKey = stringWriter.ToString();

        // Private key.
        var privKey = csp.ExportParameters( true );

        stringWriter = new StringWriter();
        serializer = new XmlSerializer( typeof( RSAParameters ) );

        serializer.Serialize( stringWriter, privKey );
        privateKey = stringWriter.ToString();

        // Done.
        return true;
    }
    public void GenerarNuevaLlave(ref string llavePubPriXml, ref string llavePubXml, string semilla = "")
    {
        try
        {
            int PROVIDER_RSA_FULL = 1;
            string CONTAINER_NAME = semilla + DateTime.Now.Ticks.ToString();
            CspParameters cspParams;
            cspParams = new CspParameters( PROVIDER_RSA_FULL );
            cspParams.KeyContainerName = CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            rsa = new RSACryptoServiceProvider( cspParams );

            rsa.PersistKeyInCsp = false;

            //Pair of public and private key as XML string.
            //Do not share this to other party
            llavePubPriXml = rsa.ToXmlString( true );

            //Private key in xml file, this string should be share to other parties
            llavePubXml = rsa.ToXmlString( false );

            //RSAParameters RSAParams = rsa.ExportParameters(true);

        }
        catch (Exception ex)
        {
            throw new Exception( "GenRsaPublicaPrivada.GenerarNuevaLlave: " + ex );
        }
    }

    public string Decrypt(string llavePubPriXml, byte[] datosEncriptados)
    {
        try
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString( llavePubPriXml );

            return ASCIIEncoding.UTF8.GetString( rsa.Decrypt( datosEncriptados, true ) );
        }
        catch (Exception ex)
        {
            throw new Exception( "GenRsaPublicaPrivada.Decrypt: " + ex );
        }
    }

    public byte[] Encrypt(string llavePubXml, string datosAEncriptar)
    {
        try
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString( llavePubXml );

            return rsa.Encrypt( ASCIIEncoding.UTF8.GetBytes( datosAEncriptar ), true );
        }
        catch (Exception ex)
        {
            throw new Exception( "GenRsaPublicaPrivada.Encrypt: " + ex );
        }
    }
}
