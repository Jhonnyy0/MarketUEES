@echo off
setlocal EnableExtensions

if "%~1"=="" (
  echo Uso: download-cassandra-minio.cmd backup_minio_YYYYMMDD_HHMMSS
  echo Para ver nombres ejecuta:
  echo docker exec cassandra-backup-minio mc ls localminio/cassandra-backups
  exit /b 1
)

set BACKUP=%~1

mkdir Backups_Minio_MarketUEES 2>nul

docker exec cassandra-backup-minio mc alias set localminio http://minio:9000 admin admin123
docker exec cassandra-backup-minio sh -c "test -d /minio-work"
if errorlevel 1 (
  echo.
  echo El volumen /minio-work no esta montado correctamente.
  echo Ejecuta:
  echo docker compose up -d --force-recreate --remove-orphans
  exit /b 1
)
docker exec cassandra-backup-minio sh -c "mkdir -p /minio-work/%BACKUP%"
docker exec cassandra-backup-minio mc cp --recursive localminio/cassandra-backups/%BACKUP%/ /minio-work/%BACKUP%/
if errorlevel 1 exit /b 1

echo.
echo Backup descargado en: Backups_Minio_MarketUEES\%BACKUP%
echo.
dir Backups_Minio_MarketUEES\%BACKUP%
