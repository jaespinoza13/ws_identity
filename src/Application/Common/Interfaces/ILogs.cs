using Application.Common.ISO20022.Models;
using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface ILogs
{
    Task SaveHeaderLogs(dynamic transaction, String str_operacion, String str_metodo, String str_clase);
    Task SaveResponseLogs(dynamic transaction, String str_operacion, String str_metodo, String str_clase);
    Task SaveExceptionLogs(dynamic transaction, String str_operacion, String str_metodo, String str_clase, Object obj_error);
    Task SaveAmenazasLogs(ValidacionInyeccion validacion, String str_operacion, String str_metodo, String str_clase);
    Task SaveHttpErrorLogs(dynamic transaction, String str_metodo, String str_clase, dynamic obj_error, String str_id_transaccion);
    Task SaveExcepcionDataBaseSybase( dynamic transaction, String str_metodo, Exception excepcion, string str_clase);
}