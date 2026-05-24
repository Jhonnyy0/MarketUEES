@echo off
setlocal EnableExtensions

if "%~1"=="" (
  echo Uso: restore-cassandra-minio.cmd backup_minio_YYYYMMDD_HHMMSS
  echo Para ver nombres ejecuta:
  echo docker exec cassandra-backup-minio mc ls localminio/cassandra-backups
  exit /b 1
)

call download-cassandra-minio.cmd %~1
if errorlevel 1 exit /b 1

call restore-cassandra.cmd Backups_Minio_MarketUEES\%~1
if errorlevel 1 exit /b 1
