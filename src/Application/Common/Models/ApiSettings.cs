namespace Application.Common.Models
{
    public class ApiSettings
    {
        public string? DB_meg_megonline { get; set; }
        public string? DB_meg_servicios { get; set; }
        public string? DB_meg_sistemas { get; set; }
        public string? DB_meg_appmovil { get; set; }


        public string? client_grpc_sybase { get; set; }
        public string? client_grpc_mongo { get; set; }
        public int timeoutGrpcSybase { get; set; }
        public int delayOutGrpcSybase { get; set; }
        public int timeoutGrpcMongo { get; set; }
        public int delayOutGrpcMongo { get; set; }


        public string nombre_base_mongo { get; set; } = String.Empty;
        public string coll_peticiones { get; set; } = String.Empty;
        public string coll_errores { get; set; } = String.Empty;
        public string coll_amenazas { get; set; } = String.Empty;
        public string coll_respuesta { get; set; } = String.Empty;
        public string coll_peticiones_diarias { get; set; } = String.Empty;
        public string coll_promedio_peticiones_diarias { get; set; } = String.Empty;
        public string coll_errores_db { get; set; } = String.Empty;
        public string coll_errores_http { get; set; } = String.Empty;


        public string logs_path_peticiones { get; set; } = String.Empty;
        public string logs_path_errores { get; set; } = String.Empty;
        public string logs_path_errores_db { get; set; } = String.Empty;
        public string logs_path_amenazas { get; set; } = String.Empty;
        public string logs_path_errores_http { get; set; } = String.Empty;


        public List<int> lst_codigos_error_sistemas { get; set; } = new ();
        public List<string> lst_nombres_parametros { get; set; } = new ();
        public List<string> lst_canales_encriptar { get; set; } = new ();

        public bool valida_peticiones_diarias { get; set; }
        public int timeOutHttp { get; set; }


        public string servicio_ws_otp { get; set; } = String.Empty;
        public string servicio_encrypt { get; set; } = String.Empty;
        public string url_logs { get; set; } = String.Empty;


        public string auth_ws_identity { get; set; } = String.Empty;
        public string auth_ws_otp { get; set; } = String.Empty;
        public string auth_logs { get; set; } = String.Empty;

        public bool valor_encriptar { get; set; }

        public int mostrar_descripcion_badrequest { get; set; }

        public string url_acceso_logs { get; set; } = String.Empty;

        #region CollectionsMongo
        public string errores_http { get; set; } = "errores_http";
        #endregion

        #region TypesAuthorization
        public string typeAuthAccesoLogs { get; set; } = String.Empty;
        #endregion

    }
}