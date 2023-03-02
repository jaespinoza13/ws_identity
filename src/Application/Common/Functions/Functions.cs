
using System.Security.Cryptography;
using System.Text;
using Application.LogInMegomovil;

namespace Application.Common.Functions;

internal static class Functions
{
    private static readonly byte[] s_secp256r1Prefix = Convert.FromBase64String("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE");
    public static bool ValidarClave ( string claveUsuario, string claveBase )
    {
        return BCrypt.Net.BCrypt.Verify(claveUsuario, claveBase);
    }

    public static bool VerificarHuella ( ReqValidarLogin req_validar_login, string str_clave_publica )
    {
        bool bln_respuesta = false;
        try
        {
            byte[] publicKeyBytes = Convert.FromBase64String(str_clave_publica);
            //Pasar clave publica generada en el dispositivo a un formato legible en este lenguaje
            var keyType = new byte[] { 0x45, 0x43, 0x53, 0x31 };
            var keyLength = new byte[] { 0x20, 0x00, 0x00, 0x00 };
            var ultimos64 = publicKeyBytes.Skip(Math.Max(0, publicKeyBytes.Count( ) - 64));
            var key = keyType.Concat(keyLength).Concat(ultimos64).ToArray( );

            //Firma proveniente del dispositivo y pasada a un formato legible
            var firma = Convert.FromBase64String(req_validar_login.str_firma_digital!);
            var int_logitud_r = firma[3];
            byte[] r = new byte[int_logitud_r];
            Array.Copy(firma, 4, r, 0, int_logitud_r);
            var int_posicion_s = 4 + int_logitud_r + 1;
            var int_logitud_s = firma[int_posicion_s];
            byte[] s = new byte[int_logitud_s];
            Array.Copy(firma, int_posicion_s + 1, s, 0, int_logitud_s);
            var int_indice_ini_r = 0;
            var int_indice_fin_r = 0;
            var int_lon_valida_r = r.Length;
            var int_indice_ini_s = 0;
            var int_indice_fin_s = 0x20;
            var int_lon_valida_s = s.Length;
            if (r.Length != 0x20)
            {
                int_logitud_r = 0x20;
                if (r.Length > 0x20)
                {
                    int_indice_ini_r = 1;
                    int_lon_valida_r -= 1;
                }
                else
                {
                    int_indice_ini_r = 0;
                    int_indice_fin_r += 1;
                }
            }
            if (s.Length != 0x20)
            {
                int_logitud_s = 0x20;
                if (s.Length > 0x20)
                {
                    int_indice_ini_s = 1;
                    int_lon_valida_s -= 1;
                }
                else
                {
                    int_indice_ini_s = 0;
                    int_indice_fin_s += 1;
                }
            }
            var signature = new byte[int_logitud_r + int_logitud_s];
            Array.Copy(r, int_indice_ini_r, signature, int_indice_fin_r, int_lon_valida_r);
            Array.Copy(s, int_indice_ini_s, signature, int_indice_fin_s, int_lon_valida_s);

            var str_datos = new StringBuilder( );
            str_datos.Append(req_validar_login.str_identificador);
            str_datos.Append(req_validar_login.str_login);
            str_datos.Append(req_validar_login.str_password);

            var data = Encoding.UTF8.GetBytes(str_datos.ToString( ));

            ECParameters ecParameters = ConvertSecp256r1PublicKeyToECParameters(str_clave_publica);     // Replaced!

            // Verify the signature
            var eCDsa = ECDsa.Create( );
            eCDsa.ImportParameters(ecParameters);
            bln_respuesta = eCDsa.VerifyData(data, signature, HashAlgorithmName.SHA256);


        }
        catch (Exception ex)
        {
            throw new Exception("Error al verificar huella " + ex);
        }
        return bln_respuesta;
    }    

    private static ECParameters ConvertSecp256r1PublicKeyToECParameters ( string base64 )
    {
        byte[] subjectPublicKeyInfo = Convert.FromBase64String(base64);

        if (subjectPublicKeyInfo.Length != 91)
            throw new InvalidOperationException( );

        byte[] prefix = s_secp256r1Prefix;

        if (!subjectPublicKeyInfo.Take(prefix.Length).SequenceEqual(prefix))
            throw new InvalidOperationException( );

        byte[] x = new byte[32];
        byte[] y = new byte[32];
        Buffer.BlockCopy(subjectPublicKeyInfo, prefix.Length, x, 0, x.Length);
        Buffer.BlockCopy(subjectPublicKeyInfo, prefix.Length + x.Length, y, 0, y.Length);

        ECParameters ecParameters = new ECParameters( );
        ecParameters.Curve = ECCurve.NamedCurves.nistP256; // aka secp256r1 aka  prime256v1
        ecParameters.Q.X = x;
        ecParameters.Q.Y = y;

        return ecParameters;
    }

}
