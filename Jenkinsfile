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
                            
                            # Parantez gruplaması ve derinlik sınırı olmadan tam arama
                            TARGET=$(find . \\( -name "*.sln" -o -name "*.csproj" \\) -not -path "*/obj/*" -not -path "*/bin/*" | head -n 1)
                            
                            if [ -z "$TARGET" ]; then
                                echo "HATA: Proje dosyası (.sln/.csproj) bulunamadı!"
                                echo "Mevcut dizin içeriği:"
                                ls -la
                                exit 1
                            fi
                            
                            echo "Bulunan Proje Dosyası: $TARGET"
                            
                            dotnet-sonarscanner begin \
                              /k:"AssetManagementApp" \
                              /d:sonar.host.url="http://sonarqube:9000" \
                              /d:sonar.token="$SONAR_TOKEN"
                            
                            dotnet build "$TARGET" --configuration Release
                            
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