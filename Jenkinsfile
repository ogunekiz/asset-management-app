pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                echo 'Kod depodan çekiliyor...'
                checkout scm
            }
        }

        stage('SAST - Semgrep Güvenlik Taraması') {
            steps {
                echo 'Semgrep SAST güvenlik taraması başlatılıyor...'
                sh '''
                    docker run --rm -v "$(pwd):/src" returntocorp/semgrep semgrep scan --config=auto /src || true
                '''
            }
        }

        stage('SonarQube Analizi') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    sh '''
                        docker run --rm \
                          --network devsecops-net \
                          --env SONAR_TOKEN="${SONAR_AUTH_TOKEN}" \
                          -v "$(pwd):/app" \
                          -w /app \
                          mcr.microsoft.com/dotnet/sdk:9.0 bash -c '
                            set -e
                            dotnet tool install --global dotnet-sonarscanner || true
                            export PATH="$PATH:/root/.dotnet/tools"
                            
                            # Workspace icindeki .sln/.csproj barindiran klasoru tespit edip icine giriyoruz
                            PROJ_PATH=$(find /app -name "*.sln" -o -name "*.csproj" | head -n 1)
                            
                            if [ -z "$PROJ_PATH" ]; then
                                echo "HATA: Workspace icinde hiçbir .sln veya .csproj bulunamadi!"
                                echo "Workspace tum dosya yapisi:"
                                find /app -maxdepth 3
                                exit 1
                            fi
                            
                            PROJ_DIR=$(dirname "$PROJ_PATH")
                            PROJ_FILE=$(basename "$PROJ_PATH")
                            
                            echo "Proje konumu tespit edildi: $PROJ_DIR"
                            echo "Proje dosyasi: $PROJ_FILE"
                            
                            cd "$PROJ_DIR"
                            
                            dotnet-sonarscanner begin \
                              /k:"AssetManagementApp" \
                              /d:sonar.host.url="http://sonarqube:9000" \
                              /d:sonar.token="$SONAR_TOKEN"
                            
                            dotnet build "$PROJ_FILE" --configuration Release
                            
                            dotnet-sonarscanner end /d:sonar.token="$SONAR_TOKEN"
                          '
                    '''
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