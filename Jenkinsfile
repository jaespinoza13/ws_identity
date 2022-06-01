pipeline {
    
    agent {
        node {
            label 'web-service-development-server'
        }
    }

    environment {
        VERSION_PRODUCCION  = '3.0.0'
        VERSION_ACTUAL      = '1.0.0'
        NOMBRE_CONTENEDOR   = 'servicio-identity-des'
        NOMBRE_IMAGEN       = 'ws_identity'
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
                        -v /app/wsIdentity:/app/Logs/ ws_acceso:${VERSION_PRODUCCION}
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
