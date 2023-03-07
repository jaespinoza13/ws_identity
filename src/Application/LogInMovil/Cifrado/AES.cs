using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.LogInMegomovil
{
    public class AES
    {
        /// <summary>
        /// Encripta utilizando AES un string, dado la clave y el vector de inicialización en arreglo de bytes
        /// </summary>
        /// <param name="str_texto_plano"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] EncryptStringToBytes_Aes ( string str_texto_plano, byte[] Key, byte[] IV )
        {
            // Verifica los argumentos.
            if (str_texto_plano == null || str_texto_plano.Length <= 0)
                throw new ArgumentNullException("str_texto_plano");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encriptado;
            // Crea un objeto AES con la clave y el vector de inicialización especificados 
            using (Aes aesAlg = Aes.Create( ))
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;
                // Crea un descifrador para realizar la transformación de la secuencia.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Crea un flujo usado para el cifrado
                using (MemoryStream msEncrypt = new MemoryStream( ))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Escribe todos los datos en el flujo
                            swEncrypt.Write(str_texto_plano);
                        }
                        encriptado = msEncrypt.ToArray( );
                    }
                }
            }
            //Retorna el arreglo de bytes desde MemoryStream
            return encriptado;

        }

        public static string DecryptStringFromBytes_Aes ( byte[] bt_texto_cifrado, byte[] Key, byte[] IV )
        {
            //Verifica los argumentos
            if (bt_texto_cifrado == null || bt_texto_cifrado.Length <= 0)
                throw new ArgumentNullException("bt_texto_cifrado");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            //Declara el string usado para contener el texto descifrado
            string str_texto_plano = null!;

            //Crea un objeto AES con la clave y el vector de inicialización especificados
            using (Aes aesAlg = Aes.Create( ))
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;
                //Crea un descifrador para realizar la transformación del flujo.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                //Crea el flujo usado para descifrar
                using (MemoryStream msDecrypt = new MemoryStream(bt_texto_cifrado))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            //Lee los bytes descifrados desde el flujo descifrado y los pone en el sting
                            str_texto_plano = srDecrypt.ReadToEnd( );
                        }
                    }
                }
            }
            return str_texto_plano;

        }
    }
}
