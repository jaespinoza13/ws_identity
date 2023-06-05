using Application.Common.Converting;
using Application.Common.Interfaces;
using Application.Common.ISO20022.Models;
using Application.Common.Models;
using Domain.Entities;

namespace Application.LogInMegomovil
{
    public class CifradoMegomovil: ICifradoMegomovil
    {
        private readonly IKeysMovilDat _keysMegomovil;

        private byte[] bt_llave_desencriptada_bytes = new byte[0];
        private byte[] bt_iv = new byte[0];
        public CifradoMegomovil ( IKeysMovilDat keysMegomovil )
        {
            this._keysMegomovil = keysMegomovil;
        }
        public void getLlavesCifrado ( Header header, string str_identificador , string str_clave_secreta )
        {
            string srt_llave_pub_pri_xml = "";
            string str_iv = "";
            getLlavesCifradoCanal(header, str_identificador, ref srt_llave_pub_pri_xml, ref str_iv);

            var bt_llave_encriptada = Convert.FromBase64String(str_clave_secreta);
            var str_llave_desencriptada_str = FuncionesCifrado.Decrypt(srt_llave_pub_pri_xml, bt_llave_encriptada);
            bt_llave_desencriptada_bytes = Convert.FromBase64String(str_llave_desencriptada_str);
            bt_iv = FuncionesCifrado.HexToByteArray(str_iv);
        }

        /// <summary>
        /// Desencripta un campo string
        /// </summary>
        /// <param name="str_campo"></param>
        /// <returns></returns>
        public string desencriptarTrama ( string str_campo )
        {
            try
            {
                if (!string.IsNullOrEmpty(str_campo))
                {
                    var bt_texto_cifrado = Convert.FromBase64String(str_campo);
                    return AES.DecryptStringFromBytes_Aes(bt_texto_cifrado, bt_llave_desencriptada_bytes, bt_iv);
                }
                return str_campo;
            }
            catch (Exception ex)
            {
                throw new Exception("desencriptar_campo " + ex);
            }
        }

        /// <summary>
        /// Encriptar un campo string
        /// </summary>
        /// <param name="str_campo"></param>
        /// <returns></returns>
        public string encriptarTrama ( string str_campo )
        {
            try
            {
                if (!string.IsNullOrEmpty(str_campo))
                {
                    var bt_texto_cifrado = AES.EncryptStringToBytes_Aes(str_campo, bt_llave_desencriptada_bytes, bt_iv);
                    return Convert.ToBase64String(bt_texto_cifrado);
                }
                return str_campo;
            }
            catch (Exception ex)
            {
                throw new Exception("encriptar_campo " + ex);
            }
        }

        private void getLlavesCifradoCanal ( Header header, string str_identificador, ref string srt_llave_pub_pri_xml, ref string str_iv )
        {
            var res_tran = _keysMegomovil.getLLavesCifradoMovil(header, str_identificador);

            if (res_tran.codigo != "000")
            {
                throw new Exception( header.str_id_transaccion + " " + res_tran.diccionario["str_error"]);
            }
            var dt = Conversions.ConvertConjuntoDatosToClass<LlavesCifradoMovil>((ConjuntoDatos)res_tran.cuerpo);

            srt_llave_pub_pri_xml = dt.str_llv_pub_priv;
            str_iv = dt.str_iv;
        }
    }
}
