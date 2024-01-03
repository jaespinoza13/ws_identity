
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Common.Behaviours;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace Infrastructure.ExternalAPIs;

internal class EncryptMego : IEncryptMego
{
    private readonly IHttpService _httpService;
    private readonly ApiSettings _settings;
    private readonly ILogs _logs;
    private readonly string _str_clase;
    public EncryptMego ( IHttpService httpService, IOptionsMonitor<ApiSettings> option, ILogs logs )
    {
        _httpService = httpService;
        _settings = option.CurrentValue;
        _logs = logs;
        _str_clase = GetType( ).FullName!;
    }

    public async Task<string> Encrypt ( string str_login, string str_password, string str_id_transaccion )
    {
        try
        {
            var objData = new
            {
                str_login = str_login,
                str_password = str_password
            };

            string str_data = JsonSerializer.Serialize(objData);

            HashCobis hashCobis = await _httpService.PostRestServiceDataAsync<HashCobis>(str_data, _settings.servicio_encrypt, String.Empty, String.Empty, String.Empty, str_id_transaccion, false);
            
            return hashCobis.str_hash_password!;
        }
        catch ( Exception ex )
        {
            throw new ArgumentException(ex.ToString());
            
        }
    }
}
