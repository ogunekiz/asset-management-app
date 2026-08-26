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
                          -v "$(pwd):/app" \
                          -w /app \
                          mcr.microsoft.com/dotnet/sdk:9.0 sh -c "
                            dotnet tool install --global dotnet-sonarscanner
                            export PATH=\\$PATH:/root/.dotnet/tools
                            
                            # Proje veya solution dosyasının yerini derinlemesine buluyoruz
                            TARGET_FILE=\\$(find . -name '*.sln' -o -name '*.csproj' | head -n 1)
                            
                            if [ -z \\"\$TARGET_FILE\\" ]; then
                                echo \\"HATA: Repoda .sln veya .csproj dosyasi bulunamadi!\\"
                                exit 1
                            fi

                            TARGET_DIR=\\$(dirname \\"\$TARGET_FILE\\")
                            FILE_NAME=\\$(basename \\"\$TARGET_FILE\\")
                            
                            echo \\"Bulunan Proje Dosyasi: \$FILE_NAME (Dizin: \$TARGET_DIR)\\"
                            
                            cd \\"\$TARGET_DIR\\"
                            
                            dotnet-sonarscanner begin /k:\\"AssetManagementApp\\" /d:sonar.host.url=\\"http://sonarqube:9000\\" /d:sonar.token=\\"$SONAR_AUTH_TOKEN\\"
                            dotnet build \\"\$FILE_NAME\\" --configuration Release
                            dotnet-sonarscanner end /d:sonar.token=\\"$SONAR_AUTH_TOKEN\\"
                          "
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