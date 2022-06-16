namespace Application.Common.Models;
public class InfoLog
{
    /// <summary>
    /// Fecha de transacción
    /// </summary>
    public DateTime str_fecha { get; set; }
    /// <summary>
    /// Id de la transacción
    /// </summary>
    public string str_id_transaccion { get; set; } = String.Empty;
    /// <summary>
    /// Tipo de log. 
    /// s>: = Solicitud transacción, r<: = Respuesta transaccion y  ex<: = Excepción transacción.
    /// </summary>
    public string str_tipo { get; set; } = String.Empty;
    /// <summary>
    /// Web service desde el cual se está realizando la transacción
    /// </summary>
    public string str_webservice { get; set; } = String.Empty;
    /// <summary>
    /// Clase del Web Service desde el cual se está realizando la transacción
    /// </summary>
    public string? str_clase { get; set; }
    /// <summary>
    /// Metodo de la clase en la cual se está realizando la transacción
    /// </summary>
    public string str_metodo { get; set; } = String.Empty;

    /// <summary>
    /// Nombre de la operación o proceso que se esté realizando 
    /// </summary>
    public string str_operacion { get; set; } = String.Empty;
    /// <summary>
    /// Objeto de transacción (iso20022 o genérico)
    /// </summary>
    public object str_objeto { get; set; } = new ();
}