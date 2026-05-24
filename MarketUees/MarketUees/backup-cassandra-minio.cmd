@echo off
setlocal EnableExtensions

set LOCK_DIR=Backup_Lock_MarketUEES
set LOCK_FILE=%LOCK_DIR%\backup.lock
mkdir "%LOCK_DIR%" 2>nul

if exist "%LOCK_FILE%" (
  echo Ya hay un backup en proceso. Espera a que termine antes de crear otro.
  exit /b 1
)

echo backup_minio > "%LOCK_FILE%"

for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss"') do set TAG=backup_minio_%%i
set DEST=Backups_Minio_MarketUEES\%TAG%

mkdir Backups_Minio_MarketUEES 2>nul

docker exec cassandra-node1 cqlsh -e "DESCRIBE KEYSPACE marketuees" >nul 2>nul
if errorlevel 1 (
  echo Primero levanta la API para que cree el keyspace marketuees.
  del "%LOCK_FILE%" 2>nul
  exit /b 1
)

mkdir "%DEST%" 2>nul

docker exec cassandra-node1 mkdir -p /tmp/%TAG%
docker exec cassandra-node1 cqlsh -e "COPY marketuees.vistas_contenido TO '/tmp/%TAG%/vistas_contenido.csv' WITH HEADER = TRUE;"
if errorlevel 1 goto error
docker exec cassandra-node1 cqlsh -e "COPY marketuees.actividad_usuario TO '/tmp/%TAG%/actividad_usuario.csv' WITH HEADER = TRUE;"
if errorlevel 1 goto error
docker cp cassandra-node1:/tmp/%TAG%/. "%DEST%"
if errorlevel 1 goto error
docker exec cassandra-node1 rm -rf /tmp/%TAG%

docker exec cassandra-backup-minio mc alias set localminio http://minio:9000 admin admin123
if errorlevel 1 goto error
docker exec cassandra-backup-minio mc mb -p localminio/cassandra-backups
docker exec cassandra-backup-minio mc cp --recursive /minio-work/%TAG%/ localminio/cassandra-backups/%TAG%/
if errorlevel 1 goto error
docker exec cassandra-backup-minio sh -c "touch /minio-work/%TAG%/.uploaded"
del "%LOCK_FILE%" 2>nul

echo.
echo Backup MinIO creado: %TAG%
echo.
docker exec cassandra-backup-minio mc ls localminio/cassandra-backups
exit /b 0

:error
docker exec cassandra-node1 rm -rf /tmp/%TAG% >nul 2>nul
del "%LOCK_FILE%" 2>nul
echo Error creando backup MinIO. No se borraron datos de Cassandra.
exit /b 1
