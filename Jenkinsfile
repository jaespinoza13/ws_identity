pipeline {
    
    agent {
        node {
            label 'web-service-development-server'
        }
    }

    environment {
        VERSION_PRODUCCION  = '3.0.0'
        VERSION_ACTUAL      = '1.0.0'
        NOMBRE_CONTENEDOR   = 'api-identity-qa'
        NOMBRE_IMAGEN       = 'ws_identity'
        PUERTO              = '9016'
        PUERTO_CONTENEDOR   = '80'
        RUTA_LOGS           = '/app/wsIdentity'
    }

    stages {


        stage('Deploy') {
            steps {
                echo 'Deploying....'
                sh  '''docker run --restart=always -it -dp ${PUERTO}:${PUERTO_CONTENEDOR} \
                        --name ${NOMBRE_CONTENEDOR} \
                        -v ${RUTA_LOG}:/app/Logs/  \
                        -e TZ=${TZ} \
                        -e Key_canbvi=${SECRETKEY} \
                        -e Key_bmo=${SECRETKEY} \
                        -e Key_bim=${SECRETKEY} \
                        -e Issuer=${ISSUER} \
                        -e ApiSettings__GrpcSettings__client_grpc_sybase=${ENDPOINT_GRPC_SYBASE_QA} \
                        -e ApiSettings__GrpcSettings__client_grpc_mongo=${ENDPOINT_GRPC_MONGO} \
                        -e ApiSettings__Endpoints__servicio_ws_otp=${ENDPOINT_WS_OTP} \
                        -e ApiSettings__Endpoints__servicio_encrypt=${ENDPOINT_ENCRYPT_COBIS} \
                        -e ApiSettings__EndpointsAuth__auth_ws_otp=${AUTH_WS_OTP} \
                        -e ApiSettings__EndpointsAuth__auth_ws_identity=${AUTH_WS_IDENTITY} \
                        ${NOMBRE_IMAGEN}:${VERSION_PRODUCCION}
                    '''
            }
        }
        stage('Restart') {
            steps {
                echo 'Deploying....'
                sh 'docker restart ${NOMBRE_CONTENEDOR}'
            }
        }
    }
}
