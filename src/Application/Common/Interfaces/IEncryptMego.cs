
namespace Application.Common.Interfaces;

public interface IEncryptMego
{
    Task<string> Encrypt ( string str_login, string str_password, string str_id_transaccion );
}
