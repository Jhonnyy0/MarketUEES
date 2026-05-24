@echo off
setlocal EnableExtensions

for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss"') do set TAG=backup_minio_%%i
set DEST=Backups_Minio_MarketUEES\%TAG%

mkdir Backups_Minio_MarketUEES 2>nul

docker exec cassandra-node1 cqlsh -e "DESCRIBE KEYSPACE marketuees" >nul 2>nul
if errorlevel 1 (
  echo Primero levanta la API para que cree el keyspace marketuees.
  exit /b 1
)

mkdir "%DEST%" 2>nul

docker exec cassandra-node1 mkdir -p /tmp/%TAG%
docker exec cassandra-node1 cqlsh -e "COPY marketuees.vistas_contenido TO '/tmp/%TAG%/vistas_contenido.csv' WITH HEADER = TRUE;"
docker exec cassandra-node1 cqlsh -e "COPY marketuees.actividad_usuario TO '/tmp/%TAG%/actividad_usuario.csv' WITH HEADER = TRUE;"
docker cp cassandra-node1:/tmp/%TAG%/. "%DEST%"
docker exec cassandra-node1 rm -rf /tmp/%TAG%

docker exec cassandra-backup-minio mc alias set localminio http://minio:9000 admin admin123
docker exec cassandra-backup-minio mc mb -p localminio/cassandra-backups
docker exec cassandra-backup-minio mc cp --recursive /minio-work/%TAG%/ localminio/cassandra-backups/%TAG%/
if errorlevel 1 exit /b 1
docker exec cassandra-backup-minio sh -c "touch /minio-work/%TAG%/.uploaded"

echo.
echo Backup MinIO creado: %TAG%
echo.
docker exec cassandra-backup-minio mc ls localminio/cassandra-backups
