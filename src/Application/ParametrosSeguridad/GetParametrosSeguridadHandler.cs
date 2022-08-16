using System.Reflection;

using Application.Common.Models;
using Application.Common.Converting;
using Application.Common.Interfaces;
using MediatR;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Application.ParametrosSeguridad;
public class GetParametrosSeguridadHandler : IRequestHandler<ReqGetParametrosSeguridad, ResGetParametrosSeguridad>
{
    public readonly ILogs _logsService;
    private readonly string str_clase;
    public readonly IMemoryCache _memoryCache;


    public GetParametrosSeguridadHandler ( ILogs logsService, IMemoryCache memoryCacheLlave )
    {
        this._logsService = logsService;
        this.str_clase = GetType().FullName!;
        this._memoryCache = memoryCacheLlave;
    }

    public async Task<ResGetParametrosSeguridad> Handle(ReqGetParametrosSeguridad reqGetParametrosSeguridad, CancellationToken cancellationToken)
    {
        string str_operacion = "GET_PARAMETROS_SEGURIDAD";
        var respuesta = new ResGetParametrosSeguridad();
        respuesta.LlenarResHeader( reqGetParametrosSeguridad );
        await _logsService.SaveHeaderLogs( reqGetParametrosSeguridad, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase );

        try
        {

            var date = _memoryCache.Get<DateTime>("Date");
            if (DateTime.Compare(DateTime.Now, date.AddHours(1)) > 0)
            {
                var KeyCreate = CifradoRSA.GenerarLlavePublicaPrivada(reqGetParametrosSeguridad.str_nemonico_canal);
                _memoryCache.Set<DatosLlaveRsa>("Key", KeyCreate);
                _memoryCache.Set<DateTime>("Date", DateTime.Now);
            }
            var Key = _memoryCache.Get<DatosLlaveRsa>("Key");
            respuesta.datos_llave.str_modulo = Key.str_modulo;
            respuesta.datos_llave.str_exponente = Key.str_exponente;


            await _logsService.SaveResponseLogs( respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase );
            return respuesta;
        }
        catch (Exception exception)
        {
            await _logsService.SaveExceptionLogs( respuesta, str_operacion, MethodBase.GetCurrentMethod()!.Name, str_clase, exception );
            throw new ArgumentException( reqGetParametrosSeguridad.str_id_transaccion )!;
        }
    }
    public static string Decrypt(string input, string key)
    {
        var stringReader = new StringReader( key );
        var serializer = new XmlSerializer( typeof( RSAParameters ) );
        var deskey = (RSAParameters)serializer.Deserialize( stringReader );

        var bytes = Decrypt(
            Convert.FromBase64String( input ),
            deskey );

        return Encoding.UTF8.GetString( bytes );
    }
    public static byte[] Decrypt(byte[] input, RSAParameters key)
    {
        using var rsa = RSA.Create( key );

        var bytes = rsa.Decrypt(
            input,
            RSAEncryptionPadding.Pkcs1 );

        return bytes;
    }
    public static string Encrypt(string input, string key)
    {
        var stringReader = new StringReader( key );
        var serializer = new XmlSerializer( typeof( RSAParameters ) );
        var deskey = (RSAParameters)serializer.Deserialize( stringReader );

        var bytes = Encrypt(
            Encoding.UTF8.GetBytes( input ),
            deskey );

        return Convert.ToBase64String( bytes );
    }
    public static byte[] Encrypt(byte[] input, RSAParameters key)
    {
        using var rsa = RSA.Create( key );

        var bytes = rsa.Encrypt(
            input,
            RSAEncryptionPadding.Pkcs1);

        return bytes;
    }
}