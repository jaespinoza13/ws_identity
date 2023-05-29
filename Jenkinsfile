pipeline {
    
    agent {
        node {
            label 'web-service-production-server'
        }
    }

    environment {
        VERSION_DESPLIEGUE  = '1.1.1'
        VERSION_PRODUCCION  = '1.1.0'
        NOMBRE_CONTENEDOR   = 'servicio-identity-movil'
        NOMBRE_IMAGEN       = 'ws_identity_movil'
        PUERTO              = '8010'
        PUERTO_CONTENEDOR   = '80'
		RUTA_CONFIG 		= '/config/wsIdentity/'
        RUTA_LOGS           = '/app/wsIdentity'
    }
    
    stages {
        stage('Build') {
            steps {
                echo 'Building ...'
                sh 'docker build -t ${NOMBRE_IMAGEN}:${VERSION_DESPLIEGUE} --no-cache .'
            }
        }

        stage('Test') {
            steps {
                echo 'Testing ...'
            }
        }

        stage('Clean') {
            steps {
                echo 'Cleaning ...'
                sh 'docker rm -f ${NOMBRE_CONTENEDOR}'
            }
        }

        stage('Deploy') {
            steps {
                echo 'Deploying ...'
                sh  '''docker run --restart=always -it -dp ${PUERTO}:${PUERTO_CONTENEDOR} --name ${NOMBRE_CONTENEDOR} \
                        -e TZ=${TZ} \
						-v ${RUTA_LOGS}:/app/Logs/ \
                        -v ${RUTA_CONFIG}appsettings_preprod_migracion_bmo.json:/app/appsettings.json \
                        ${NOMBRE_IMAGEN}:${VERSION_DESPLIEGUE}
                    '''
            }
        }

        stage('Restart') {
            steps {
                echo 'Restarting ...'
                sh 'docker restart ${NOMBRE_CONTENEDOR}'
            }
        }
    }
	
	post {

        success {
            slackSend color: '#BADA55', message: "Despliegue exitoso  - ${env.JOB_NAME} versión publicada ${VERSION_DESPLIEGUE} (<${env.BUILD_URL}|Open>)"
        }

        failure {
            sh  'docker rm -f ${NOMBRE_CONTENEDOR}'
            sh  '''docker run --restart=always -it -dp ${PUERTO}:${PUERTO_CONTENEDOR} --name ${NOMBRE_CONTENEDOR} \
					-e TZ=${TZ} \
					-v ${RUTA_LOGS}:/app/Logs/ \
					-v ${RUTA_CONFIG}appsettings_preprod_migracion_bmo.json:/app/appsettings.json \
					${NOMBRE_IMAGEN}:${VERSION_PRODUCCION}
                '''
            slackSend color: '#FE2D00', failOnError:true, message:"Despliegue fallido 😬 - ${env.JOB_NAME} he reversado a la versión ${VERSION_PRODUCCION} - (<${env.BUILD_URL}|Open>)"
        }
    }
}
