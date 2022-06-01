
namespace Application.Common.Models;

public class PromedioPeticionDiaria
{
    public string _id { get; set; } = String.Empty;
    public double dbl_promedio_peticion { get; set; }
    public string str_operacion { get; set; } = String.Empty;
    public string str_fecha_actualizacion { get; set; } = String.Empty;

}