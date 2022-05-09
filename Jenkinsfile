pipeline {
    
    agent {
        node {
            label 'web-service-development-server'
        }
    }

    environment {
        VERSION_PRODUCCION  = '1.0.0'
        VERSION_ACTUAL      = '1.0.0'
        NOMBRE_CONTENEDOR   = 'servicio-token-jwt-des'
        NOMBRE_IMAGEN       = 'ws_token_jwt'
        PUERTO              = '8016'
        PUERTO_CONTENEDOR   = '80'
    }

    stages {
        
        stage('Build') {
            when {
                branch env.RAMA_PUBLICAR
            }
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
                        -e TZ=${TZ} \
                        -e SettingsJwt__SecretKey=${SECRETKEY} \
                        -e SettingsApi__Endpoints__servicio_grpc_sybase=${ENDPOINT_GRPC_SYBASE} \
                        -e SettingsApi__Endpoints__servicio_grpc_mongo=${ENDPOINT_GRPC_MONGO} \
                        -e SettingsApi__BasicAuth__auth_ws_token_jwt=${AUTH_WS_TOKEN_JWT} \
                        -v /app/wsTokenJwt:/app/Logs/ ws_token_jwt:${VERSION_PRODUCCION}
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
