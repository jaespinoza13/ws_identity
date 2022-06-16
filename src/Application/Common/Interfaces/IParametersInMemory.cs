
using Domain.Parameters;

namespace Application.Common.Interfaces;
public interface IParametersInMemory
{
    void ValidaParametros();
    void LoadConfiguration();
    Parametro FindErrorCode(string str_codigo);
    Parametro FindParametro(string str_nemonico);
}