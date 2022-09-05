pipeline {
    
 agent {
        node {
            label 'web-service-development-server'
        }
    }

    environment {
        VERSION_PRODUCCION  = '3.0.0'
        VERSION_ACTUAL      = '1.0.0'
        NOMBRE_CONTENEDOR   = 'api-identity-des-encriptar-info-lmorocho'
        NOMBRE_IMAGEN       = 'ws_identity_des_encriptar_info_lmorocho'
        PUERTO              = '5316'
        PUERTO_CONTENEDOR   = '80'
        RUTA_LOGS           = '/app/wsIdentity'
    }
    
    stages {

        stage('Build') {
            steps {
                echo 'Building..'
                sh 'docker build -t ${NOMBRE_IMAGEN}:${VERSION_PRODUCCION} --no-cache .'
            }
        }

        stage('Test') {
            steps {
                echo 'Testing..'
            }
        }

        stage('Clean') {
            steps {
                echo 'Cleaning..'
                sh 'docker rm -f ${NOMBRE_CONTENEDOR}'
            }
        }

        stage('Deploy') {
            steps {
                echo 'Deploying....'
                sh  '''docker run --restart=always -it -dp ${PUERTO}:${PUERTO_CONTENEDOR} \
                        --name ${NOMBRE_CONTENEDOR} \
                        -v ${RUTA_LOGS}:/app/Logs/  \
                        -e TZ=${TZ} \
                        -e Key_canbvi=${SECRETKEY} \
                        -e Key_canbmo=${SECRETKEY} \
                        -e Key_canbim=${SECRETKEY} \
                        -e Key_canven=${SECRETKEY} \
                        -e Key_canbvi_pri=${SECRET_KEY_TOKEN_PRI} \
                        -e Key_canbmo_pri=${SECRET_KEY_TOKEN_PRI} \
                        -e Key_canbim_pri=${SECRET_KEY_TOKEN_PRI} \
                        -e Key_canven_pri=${SECRET_KEY_TOKEN_PRI} \
                        -e Key_canbvi_encrypt_token=${SECRET_KEY_ENCRYPT_TOKEN} \
                        -e Key_canbmo_encrypt_token=${SECRET_KEY_ENCRYPT_TOKEN} \
                        -e Key_canbim_encrypt_token=${SECRET_KEY_ENCRYPT_TOKEN} \
                        -e Key_canven_encrypt_token=${SECRET_KEY_ENCRYPT_TOKEN} \
                        -e Issuer=${ISSUER} \
                        -e ApiSettings__GrpcSettings__client_grpc_sybase=${ENDPOINT_GRPC_SYBASE} \
                        -e ApiSettings__GrpcSettings__client_grpc_mongo=${ENDPOINT_GRPC_MONGO} \
                        -e ApiSettings__Endpoints__servicio_ws_otp=${ENDPOINT_WS_OTP} \
                        -e ApiSettings__Endpoints__servicio_encrypt=${ENDPOINT_WS_ENCRYPT} \
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
