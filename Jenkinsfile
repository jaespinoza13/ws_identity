pipeline {
    
    agent {
        node {
            label 'web-service-development-server'
        }
    }

    environment {
        VERSION_PRODUCCION  = '3.0.0'
        VERSION_ACTUAL      = '1.0.0'
        NOMBRE_CONTENEDOR   = 'api-identity-des'
        NOMBRE_IMAGEN       = 'ws_identity'
        PUERTO              = '5016'
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
                        -v ${RUTA_LOG}:/app/Logs/  \
                        -e TZ=${TZ} \
                        -e Key_canbvi=${SECRETKEY} \
                        -e Key_bmo=${SECRETKEY} \
                        -e Key_bim=${SECRETKEY} \
                        -e Issuer=${ISSUER} \
                        -e ApiSettings__GrpcSettings__client_grpc_sybase=${ENDPOINT_GRPC_SYBASE} \
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
