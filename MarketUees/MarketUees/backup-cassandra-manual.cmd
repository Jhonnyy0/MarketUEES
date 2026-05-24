@echo off
setlocal EnableExtensions EnableDelayedExpansion

for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss"') do set TAG=backup_manual_%%i
set DEST=Backup_Manual_MarketUEES\%TAG%

mkdir "%DEST%" 2>nul

docker exec cassandra-node1 mkdir -p /tmp/%TAG%
docker exec cassandra-node1 cqlsh -e "COPY marketuees.vistas_contenido TO '/tmp/%TAG%/vistas_contenido.csv' WITH HEADER = TRUE;"
docker exec cassandra-node1 cqlsh -e "COPY marketuees.actividad_usuario TO '/tmp/%TAG%/actividad_usuario.csv' WITH HEADER = TRUE;"
docker cp cassandra-node1:/tmp/%TAG%/. "%DEST%"
docker exec cassandra-node1 rm -rf /tmp/%TAG%

echo.
echo Backup manual creado en: %DEST%
echo.
docker exec cassandra-node1 cqlsh -e "SELECT * FROM marketuees.vistas_contenido LIMIT 5; SELECT * FROM marketuees.actividad_usuario LIMIT 5;"
