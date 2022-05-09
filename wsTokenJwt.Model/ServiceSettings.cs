namespace wsTokenJwt.Model
{
    public class ServiceSettings
    {
        #region DATABASES SYBASE
        public string BD_meg_megonline { get; set; } = String.Empty;
        public  string BD_megsistemas { get; set; } = String.Empty;
        public  string BD_megservicios { get; set; } = String.Empty;
        #endregion

        #region ENDPOINTS
        public string servicio_grpc_sybase { get; set; } = String.Empty;
        public  string servicio_grpc_mongo { get; set; } = String.Empty;
        public string servicio_ws_token_jwt { get; set; } = String.Empty;       
        #endregion

        #region PARAMETROS ENDPOINTS
        
        public string prm_ws_acceso { get; set; } = String.Empty;
        #endregion

        #region AUTHS SERVICIOS
        public string auth_ws_token_jwt { get; set; } = String.Empty;       
        #endregion

        #region PATH DE LOGS
        public string path_logs_token_jwt { get; set; } = String.Empty;
        public  string path_logs_amenazas { get; set; } = String.Empty;
        public  string path_logs_errores { get; set; } = String.Empty;

        public  string path_logs_errores_db { get; set; } = String.Empty;
        public  string path_logs_errores_http { get; set; } = String.Empty;
        #endregion

        #region CONFIGURACION DE PETICIONES
        public int timeOutHttp{ get; set; }
        public int timeoutGrpcSybase { get; set; }
        public int delayOutGrpcSybase { get; set; }
        public int timeoutGrpcMongo { get; set; }
        public int delayOutGrpcMongo { get; set; }
        public List<string>? lst_canales_valida_token { get; set; }
        public bool valida_peticiones_diarias { get; set; }
        #endregion


        #region DATABASES MONGO
        public string nombre_base_mongo { get; set; } = String.Empty;
        public string coll_peticiones { get; set; } = String.Empty;
        public string coll_respuesta { get; set; } = String.Empty;
        public string coll_errores_db { get; set; } = String.Empty;
        public string coll_errores{ get; set; } = String.Empty;
        public string coll_amenazas{ get; set; } = String.Empty;
        public string coll_peticiones_diarias { get; set; } = String.Empty;
        public string coll_promedio_peticiones_diarias { get; set; } = String.Empty;
        public string coll_errores_http { get; set; } = String.Empty;

        #endregion

    }
}
