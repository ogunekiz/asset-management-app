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
                    docker run --rm -v "${WORKSPACE}:/src" returntocorp/semgrep semgrep scan --config=auto /src || true
                '''
            }
        }

        stage('SonarQube Analizi') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    sh '''
                        # 1. SDK Konteynirini Baslat
                        CONTAINER_ID=$(docker run -d \
                          --network devsecops-net \
                          --env SONAR_TOKEN="${SONAR_AUTH_TOKEN}" \
                          mcr.microsoft.com/dotnet/sdk:9.0 sleep 300)

                        # 2. Kodlari Konteynire Kopyala
                        docker cp . "${CONTAINER_ID}:/app"

                        # 3. Java Kurulumu, Derleme ve SonarQube Analizi
                        docker exec -w /app "${CONTAINER_ID}" bash -c '
                            set -e
                            
                            # Java (JRE 17) Kurulumu (SonarScanner post-processing icin gerekli)
                            apt-get update -y && apt-get install -y openjdk-17-jre-headless
                            
                            dotnet tool install --global dotnet-sonarscanner || true
                            export PATH="$PATH:/root/.dotnet/tools"

                            TARGET=$(find . \\( -name "*.sln" -o -name "*.csproj" \\) -not -path "*/obj/*" -not -path "*/bin/*" | head -n 1)

                            if [ -z "$TARGET" ]; then
                                echo "HATA: Proje dosyasi bulunamadi!"
                                exit 1
                            fi

                            TARGET_DIR=$(dirname "$TARGET")
                            TARGET_FILE=$(basename "$TARGET")

                            cd "$TARGET_DIR"

                            dotnet-sonarscanner begin \
                              /k:"AssetManagementApp" \
                              /d:sonar.host.url="http://sonarqube:9000" \
                              /d:sonar.token="$SONAR_TOKEN"

                            dotnet build "$TARGET_FILE" --configuration Release

                            dotnet-sonarscanner end /d:sonar.token="$SONAR_TOKEN"
                        '

                        # 4. Temizlik
                        docker rm -f "${CONTAINER_ID}"
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
            echo 'Pipeline hata aldı!'
        }
    }
}