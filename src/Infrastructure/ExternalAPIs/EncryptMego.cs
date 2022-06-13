
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Common.Behaviours;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.ExternalAPIs;

internal class EncryptMego : IEncryptMego
{
    private readonly IHttpService _httpService;
    private readonly ApiSettings _settings;
    public EncryptMego ( IHttpService httpService, IOptionsMonitor<ApiSettings> option )
    {
        _httpService = httpService;
        _settings = option.CurrentValue;
    }

    public async Task<string> Encrypt ( string str_login, string str_password, string str_id_transaccion )
    {
        var objData = new
        {
            str_login = str_login,
            str_password = str_password
        };

        string str_data = JsonSerializer.Serialize(objData);

        HashCobis hashCobis = await _httpService.PostRestServiceDataAsync<HashCobis>(str_data, _settings.servicio_encrypt, String.Empty, String.Empty, String.Empty, str_id_transaccion);
        return hashCobis.str_hash_password!;
    }
}
