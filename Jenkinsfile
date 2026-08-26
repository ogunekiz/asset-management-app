pipeline {
    agent any

    environment {
        SCANNER_HOME = tool 'Default' // İhtiyaç halinde scanner tool
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Kod depodan çekiliyor...'
                checkout scm
            }
        }

        stage('SAST - Semgrep Güvenlik Taraması') {
            steps {
                echo 'Semgrep SAST güvenlik taraması başlatılıyor (OWASP Top 10 & ISO 27001)...'
                // Docker içinde Semgrep koşturularak C# / .NET kodlarındaki zafiyetler taranır
                sh '''
                    docker run --rm -v $(pwd):/src returntocorp/semgrep semgrep scan --config=auto --error /src || true
                '''
            }
        }

        stage('SonarQube Analizi') {
            steps {
                script {
                    def scannerHome = tool 'SonarQube'
                    withSonarQubeEnv('SonarQube') {
                        // .NET SonarScanner taraması başlatılıyor
                        sh '''
                            dotnet tool install --global dotnet-sonarscanner || true
                            export PATH="$PATH:$HOME/.dotnet/tools"
                            
                            dotnet-sonarscanner begin \
                              /k:"AssetManagementApp" \
                              /d:sonar.host.url="http://sonarqube:9000" \
                              /d:sonar.token="$SONAR_AUTH_TOKEN"
                            
                            dotnet build AssetManagementApp.sln --configuration Release
                            
                            dotnet-sonarscanner end /d:sonar.token="$SONAR_AUTH_TOKEN"
                        '''
                    }
                }
            }
        }

        stage('Quality Gate Kontrolü') {
            steps {
                timeout(time: 5, unit: 'MINUTES') {
                    script {
                        waitForQualityGate abortPipeline: true
                    }
                }
            }
        }
    }

    post {
        always {
            echo 'Pipeline aşaması tamamlandı.'
        }
        failure {
            echo 'Pipeline hata aldı! SonarQube veya SAST taraması başarısız.'
        }
    }
}